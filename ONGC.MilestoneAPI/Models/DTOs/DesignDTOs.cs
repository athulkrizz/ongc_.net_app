using System.ComponentModel.DataAnnotations;
using ONGC.MilestoneAPI.Models.Enums;

namespace ONGC.MilestoneAPI.Models.DTOs;

/// <summary>
/// Request DTO for creating a design
/// </summary>
public class CreateDesignRequest
{
    /// <summary>
    /// ID of the wellbore this design belongs to
    /// </summary>
    [Required(ErrorMessage = "WellboreId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "WellboreId must be a positive number")]
    public int WellboreId { get; set; }

    /// <summary>
    /// Name of the design
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Owner/responsible person for the design
    /// </summary>
    [StringLength(200, ErrorMessage = "Owner cannot exceed 200 characters")]
    public string? Owner { get; set; }

    /// <summary>
    /// Description of the design
    /// </summary>
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }
}

/// <summary>
/// Request DTO for updating a design
/// </summary>
public class UpdateDesignRequest
{
    /// <summary>
    /// Name of the design
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Owner/responsible person for the design
    /// </summary>
    [StringLength(200, ErrorMessage = "Owner cannot exceed 200 characters")]
    public string? Owner { get; set; }

    /// <summary>
    /// Current status of the design
    /// </summary>
    public DesignStatus? Status { get; set; }

    /// <summary>
    /// Description of the design
    /// </summary>
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }
}

/// <summary>
/// Response DTO for design data
/// </summary>
public class DesignResponse
{
    /// <summary>
    /// ID of the design
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID of the wellbore this design belongs to
    /// </summary>
    public int WellboreId { get; set; }

    /// <summary>
    /// Name of the wellbore
    /// </summary>
    public string WellboreName { get; set; } = string.Empty;

    /// <summary>
    /// Name of the design
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Owner/responsible person for the design
    /// </summary>
    public string? Owner { get; set; }

    /// <summary>
    /// Date when the design was created
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Date when the design was last updated
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Current status of the design
    /// </summary>
    public DesignStatus Status { get; set; }

    /// <summary>
    /// Name of the status
    /// </summary>
    public string StatusName { get; set; } = string.Empty;

    /// <summary>
    /// Description of the design
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// When this design was created in the system
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
