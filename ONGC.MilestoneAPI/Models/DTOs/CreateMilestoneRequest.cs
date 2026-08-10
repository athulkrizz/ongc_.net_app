using System.ComponentModel.DataAnnotations;
using ONGC.MilestoneAPI.Models.Enums;

namespace ONGC.MilestoneAPI.Models.DTOs;

/// <summary>
/// Request DTO for creating a milestone
/// </summary>
public class CreateMilestoneRequest
{
    /// <summary>
    /// ID of the design this milestone belongs to
    /// </summary>
    [Required(ErrorMessage = "DesignId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "DesignId must be a positive number")]
    public int DesignId { get; set; }

    /// <summary>
    /// Type of milestone
    /// </summary>
    [Required(ErrorMessage = "MilestoneType is required")]
    public MilestoneType MilestoneType { get; set; }

    /// <summary>
    /// Timestamp when the milestone occurred
    /// </summary>
    [Required(ErrorMessage = "Timestamp is required")]
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Work centre where the milestone was achieved
    /// </summary>
    [StringLength(200, ErrorMessage = "WorkCentre cannot exceed 200 characters")]
    public string? WorkCentre { get; set; }

    /// <summary>
    /// Additional metadata in JSON format
    /// </summary>
    public string? Metadata { get; set; }
}
