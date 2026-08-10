using System.ComponentModel.DataAnnotations;

namespace ONGC.MilestoneAPI.Models.Entities;

/// <summary>
/// Represents a milestone event in the well construction process
/// </summary>
public class Milestone : BaseEntity
{
    /// <summary>
    /// Asset name (e.g., "Mumbai", "Gujarat")
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Asset { get; set; } = string.Empty;

    /// <summary>
    /// Well identifier (e.g., "MH-123")
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Well { get; set; } = string.Empty;

    /// <summary>
    /// Wellbore identifier (e.g., "MH-123-A1")
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Wellbore { get; set; } = string.Empty;

    /// <summary>
    /// User who submitted the milestone
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string User { get; set; } = string.Empty;

    /// <summary>
    /// Current milestone name/description
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string CurrentMilestone { get; set; } = string.Empty;

    /// <summary>
    /// Approval level (e.g., "Initiated", "Level-1", "Level-2", "Level-3")
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string ApprovalLevel { get; set; } = string.Empty;

    /// <summary>
    /// Status (e.g., "Data received", "In-progress", "Completed")
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Number of days for this milestone
    /// </summary>
    public int Days { get; set; }

    /// <summary>
    /// Percentage completed (0-100)
    /// </summary>
    [Range(0, 100)]
    public double PercentCompleted { get; set; }
}

