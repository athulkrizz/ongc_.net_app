using System.ComponentModel.DataAnnotations;
using ONGC.MilestoneAPI.Models.Enums;

namespace ONGC.MilestoneAPI.Models.DTOs;

/// <summary>
/// Request DTO for creating a wellbore
/// </summary>
public class CreateWellboreRequest
{
    /// <summary>
    /// ID of the well this wellbore belongs to
    /// </summary>
    [Required(ErrorMessage = "WellId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "WellId must be a positive number")]
    public int WellId { get; set; }

    /// <summary>
    /// Name/identifier of the wellbore
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Type of design for this wellbore
    /// </summary>
    [Required(ErrorMessage = "DesignType is required")]
    public DesignType DesignType { get; set; }
}

/// <summary>
/// Request DTO for updating a wellbore
/// </summary>
public class UpdateWellboreRequest
{
    /// <summary>
    /// Name/identifier of the wellbore
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Type of design for this wellbore
    /// </summary>
    [Required(ErrorMessage = "DesignType is required")]
    public DesignType DesignType { get; set; }
}

/// <summary>
/// Response DTO for wellbore data
/// </summary>
public class WellboreResponse
{
    /// <summary>
    /// ID of the wellbore
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID of the well this wellbore belongs to
    /// </summary>
    public int WellId { get; set; }

    /// <summary>
    /// Name of the well
    /// </summary>
    public string WellName { get; set; } = string.Empty;

    /// <summary>
    /// Name/identifier of the wellbore
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Type of design for this wellbore
    /// </summary>
    public DesignType DesignType { get; set; }

    /// <summary>
    /// Name of the design type
    /// </summary>
    public string DesignTypeName { get; set; } = string.Empty;

    /// <summary>
    /// When this wellbore was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
