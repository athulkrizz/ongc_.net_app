using System.ComponentModel.DataAnnotations;

namespace ONGC.MilestoneAPI.Models.DTOs;

/// <summary>
/// Request DTO for creating a well
/// </summary>
public class CreateWellRequest
{
    /// <summary>
    /// ID of the site this well belongs to
    /// </summary>
    [Required(ErrorMessage = "SiteId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "SiteId must be a positive number")]
    public int SiteId { get; set; }

    /// <summary>
    /// Name/identifier of the well
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Type of well
    /// </summary>
    [StringLength(100, ErrorMessage = "WellType cannot exceed 100 characters")]
    public string? WellType { get; set; }

    /// <summary>
    /// Current status of the well
    /// </summary>
    [StringLength(100, ErrorMessage = "Status cannot exceed 100 characters")]
    public string? Status { get; set; }
}

/// <summary>
/// Request DTO for updating a well
/// </summary>
public class UpdateWellRequest
{
    /// <summary>
    /// Name/identifier of the well
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Type of well
    /// </summary>
    [StringLength(100, ErrorMessage = "WellType cannot exceed 100 characters")]
    public string? WellType { get; set; }

    /// <summary>
    /// Current status of the well
    /// </summary>
    [StringLength(100, ErrorMessage = "Status cannot exceed 100 characters")]
    public string? Status { get; set; }
}

/// <summary>
/// Response DTO for well data
/// </summary>
public class WellResponse
{
    /// <summary>
    /// ID of the well
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID of the site this well belongs to
    /// </summary>
    public int SiteId { get; set; }

    /// <summary>
    /// Name of the site
    /// </summary>
    public string SiteName { get; set; } = string.Empty;

    /// <summary>
    /// Name/identifier of the well
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Type of well
    /// </summary>
    public string? WellType { get; set; }

    /// <summary>
    /// Current status of the well
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// When this well was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
