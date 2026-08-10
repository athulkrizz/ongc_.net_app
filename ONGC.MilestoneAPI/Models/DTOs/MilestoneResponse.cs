using ONGC.MilestoneAPI.Models.Enums;

namespace ONGC.MilestoneAPI.Models.DTOs;

/// <summary>
/// Response DTO for milestone data
/// </summary>
public class MilestoneResponse
{
    /// <summary>
    /// ID of the milestone
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID of the design this milestone belongs to
    /// </summary>
    public int DesignId { get; set; }

    /// <summary>
    /// Type of milestone
    /// </summary>
    public MilestoneType MilestoneType { get; set; }

    /// <summary>
    /// Name of the milestone type
    /// </summary>
    public string MilestoneTypeName { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the milestone occurred
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Work centre where the milestone was achieved
    /// </summary>
    public string? WorkCentre { get; set; }

    /// <summary>
    /// Additional metadata in JSON format
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// When this milestone was created in the system
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// ID of the user who created this milestone
    /// </summary>
    public int? CreatedBy { get; set; }
}
