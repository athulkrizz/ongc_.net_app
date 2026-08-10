using ONGC.MilestoneAPI.Models.Entities;

namespace ONGC.MilestoneAPI.Services.Interfaces;

/// <summary>
/// Service interface for Kafka message production
/// </summary>
public interface IKafkaProducerService
{
    /// <summary>
    /// Publish a milestone event to Kafka
    /// </summary>
    Task PublishMilestoneEventAsync(Milestone milestone, string eventId);
}

