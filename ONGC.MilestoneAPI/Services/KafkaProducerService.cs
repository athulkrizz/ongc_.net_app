using Confluent.Kafka;
using ONGC.MilestoneAPI.Models.Entities;
using ONGC.MilestoneAPI.Services.Interfaces;
using System.Text.Json;

namespace ONGC.MilestoneAPI.Services;

/// <summary>
/// Service for producing Kafka messages
/// </summary>
public class KafkaProducerService : IKafkaProducerService, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducerService> _logger;
    private const string TopicName = "milestone-events";

    public KafkaProducerService(IConfiguration configuration, ILogger<KafkaProducerService> logger)
    {
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
            ClientId = "ongc-milestone-api",
            Acks = Acks.All,
            EnableIdempotence = true,
            MaxInFlight = 5,
            MessageSendMaxRetries = 10,
            RetryBackoffMs = 100
        };

        _producer = new ProducerBuilder<string, string>(config)
            .SetErrorHandler((_, e) =>
            {
                _logger.LogError($"Kafka Error: {e.Reason}");
            })
            .Build();

        _logger.LogInformation("Kafka Producer initialized with bootstrap servers: {Servers}", config.BootstrapServers);
    }

    /// <summary>
    /// Publish a milestone event to Kafka
    /// </summary>
    public async Task PublishMilestoneEventAsync(Milestone milestone, string eventId)
    {
        try
        {
            var message = new
            {
                EventId = eventId,
                EventType = "MilestoneCreated",
                Timestamp = DateTime.UtcNow,
                Data = new
                {
                    Asset = milestone.Asset,
                    Well = milestone.Well,
                    Wellbore = milestone.Wellbore,
                    User = milestone.User,
                    CurrentMilestone = milestone.CurrentMilestone,
                    ApprovalLevel = milestone.ApprovalLevel,
                    Status = milestone.Status,
                    Days = milestone.Days,
                    PercentCompleted = milestone.PercentCompleted
                }
            };

            var messageJson = JsonSerializer.Serialize(message);
            var kafkaMessage = new Message<string, string>
            {
                Key = eventId,
                Value = messageJson
            };

            var result = await _producer.ProduceAsync(TopicName, kafkaMessage);

            _logger.LogInformation(
                "Milestone event published to Kafka. Topic: {Topic}, Partition: {Partition}, Offset: {Offset}, EventId: {EventId}",
                result.Topic, result.Partition.Value, result.Offset.Value, eventId);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Failed to publish milestone event to Kafka. EventId: {EventId}", eventId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error publishing milestone event. EventId: {EventId}", eventId);
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Flush(TimeSpan.FromSeconds(10));
        _producer?.Dispose();
    }
}
