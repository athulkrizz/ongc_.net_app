using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ONGC.MilestoneAPI.Models.Entities;
using ONGC.MilestoneAPI.Repositories.Interfaces;
using ONGC.MilestoneAPI.Services.Interfaces;
using System.Security.Claims;

namespace ONGC.MilestoneAPI.Controllers;

/// <summary>
/// Controller for managing ONGC milestone data
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MilestoneController : ControllerBase
{
    private readonly IMilestoneRepository _milestoneRepository;
    private readonly IKafkaProducerService _kafkaProducerService;
    private readonly ILogger<MilestoneController> _logger;

    public MilestoneController(
        IMilestoneRepository milestoneRepository,
        IKafkaProducerService kafkaProducerService,
        ILogger<MilestoneController> logger)
    {
        _milestoneRepository = milestoneRepository;
        _kafkaProducerService = kafkaProducerService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new milestone and publish to Kafka (Node.js will save to database)
    /// </summary>
    /// <param name="request">Milestone data</param>
    /// <returns>Accepted confirmation</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateMilestone([FromBody] CreateSimpleMilestoneRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Failed",
                Detail = "Please check the provided data.",
                Instance = HttpContext.Request.Path
            });
        }

        // Get user from token
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "Unknown";

        // Create milestone object (but don't save to database)
        var milestone = new Milestone
        {
            Asset = request.Asset,
            Well = request.Well,
            Wellbore = request.Wellbore,
            User = userEmail,
            CurrentMilestone = request.CurrentMilestone,
            ApprovalLevel = request.ApprovalLevel,
            Status = request.Status,
            Days = request.Days,
            PercentCompleted = request.PercentCompleted
        };

        // Generate a unique event ID
        var eventId = Guid.NewGuid().ToString();

        _logger.LogInformation("Publishing milestone event to Kafka: EventId={EventId}, Well={Well}", eventId, milestone.Well);

        // Publish to Kafka (Node.js consumer will save to database)
        try
        {
            await _kafkaProducerService.PublishMilestoneEventAsync(milestone, eventId);
            _logger.LogInformation("Milestone event published successfully to Kafka: EventId={EventId}", eventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish milestone to Kafka: EventId={EventId}", eventId);
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new ProblemDetails
            {
                Status = StatusCodes.Status503ServiceUnavailable,
                Title = "Service Unavailable",
                Detail = "Failed to publish event to Kafka. Please try again later.",
                Instance = HttpContext.Request.Path
            });
        }

        return Accepted(new
        {
            eventId = eventId,
            asset = milestone.Asset,
            well = milestone.Well,
            wellbore = milestone.Wellbore,
            user = milestone.User,
            currentMilestone = milestone.CurrentMilestone,
            approvalLevel = milestone.ApprovalLevel,
            status = milestone.Status,
            days = milestone.Days,
            percentCompleted = milestone.PercentCompleted,
            message = "Milestone event published to Kafka successfully. Node.js consumer will process and save it."
        });
    }

    /// <summary>
    /// Get all milestones
    /// </summary>
    /// <returns>List of milestones</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMilestones()
    {
        var milestones = await _milestoneRepository.GetAllAsync();

        var response = milestones.Select(m => new
        {
            id = m.Id,
            asset = m.Asset,
            well = m.Well,
            wellbore = m.Wellbore,
            user = m.User,
            currentMilestone = m.CurrentMilestone,
            approvalLevel = m.ApprovalLevel,
            status = m.Status,
            days = m.Days,
            percentCompleted = m.PercentCompleted,
            createdAt = m.CreatedAt
        });

        return Ok(new
        {
            count = milestones.Count(),
            data = response
        });
    }

    /// <summary>
    /// Get milestone by ID
    /// </summary>
    /// <param name="id">Milestone ID</param>
    /// <returns>Milestone details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMilestoneById(int id)
    {
        var milestone = await _milestoneRepository.GetByIdAsync(id);

        if (milestone == null)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Milestone Not Found",
                Detail = $"Milestone with ID {id} was not found.",
                Instance = HttpContext.Request.Path
            });
        }

        return Ok(new
        {
            id = milestone.Id,
            asset = milestone.Asset,
            well = milestone.Well,
            wellbore = milestone.Wellbore,
            user = milestone.User,
            currentMilestone = milestone.CurrentMilestone,
            approvalLevel = milestone.ApprovalLevel,
            status = milestone.Status,
            days = milestone.Days,
            percentCompleted = milestone.PercentCompleted,
            createdAt = milestone.CreatedAt,
            updatedAt = milestone.UpdatedAt
        });
    }

    /// <summary>
    /// Get milestones by well name
    /// </summary>
    /// <param name="wellName">Well name</param>
    /// <returns>List of milestones for the well</returns>
    [HttpGet("well/{wellName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMilestonesByWell(string wellName)
    {
        var allMilestones = await _milestoneRepository.GetAllAsync();
        var milestones = allMilestones.Where(m => m.Well.Equals(wellName, StringComparison.OrdinalIgnoreCase));

        var response = milestones.Select(m => new
        {
            id = m.Id,
            asset = m.Asset,
            well = m.Well,
            wellbore = m.Wellbore,
            user = m.User,
            currentMilestone = m.CurrentMilestone,
            approvalLevel = m.ApprovalLevel,
            status = m.Status,
            days = m.Days,
            percentCompleted = m.PercentCompleted,
            createdAt = m.CreatedAt
        });

        return Ok(new
        {
            well = wellName,
            count = milestones.Count(),
            data = response
        });
    }

    /// <summary>
    /// Delete a milestone
    /// </summary>
    /// <param name="id">Milestone ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMilestone(int id)
    {
        var milestone = await _milestoneRepository.GetByIdAsync(id);

        if (milestone == null)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Milestone Not Found",
                Detail = $"Milestone with ID {id} was not found.",
                Instance = HttpContext.Request.Path
            });
        }

        await _milestoneRepository.DeleteAsync(id);

        _logger.LogInformation("Milestone deleted: {MilestoneId}", id);

        return Ok(new
        {
            message = "Milestone deleted successfully",
            id = id
        });
    }
}

/// <summary>
/// Simple request DTO for creating a milestone
/// </summary>
public class CreateSimpleMilestoneRequest
{
    /// <summary>
    /// Asset name (e.g., "Mumbai", "Gujarat")
    /// </summary>
    public string Asset { get; set; } = string.Empty;

    /// <summary>
    /// Well identifier (e.g., "MH-123")
    /// </summary>
    public string Well { get; set; } = string.Empty;

    /// <summary>
    /// Wellbore identifier (e.g., "MH-123-A1")
    /// </summary>
    public string Wellbore { get; set; } = string.Empty;

    /// <summary>
    /// Current milestone name/description
    /// </summary>
    public string CurrentMilestone { get; set; } = string.Empty;

    /// <summary>
    /// Approval level (e.g., "Initiated", "Level-1", "Level-2", "Level-3")
    /// </summary>
    public string ApprovalLevel { get; set; } = string.Empty;

    /// <summary>
    /// Status (e.g., "Data received", "In-progress", "Completed")
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Number of days for this milestone
    /// </summary>
    public int Days { get; set; }

    /// <summary>
    /// Percentage completed (0-100)
    /// </summary>
    public double PercentCompleted { get; set; }
}
