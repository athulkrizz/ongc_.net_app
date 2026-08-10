using System.ComponentModel.DataAnnotations;

namespace ONGC.MilestoneAPI.Models.DTOs;

/// <summary>
/// Request DTO for creating a site
/// </summary>
public class CreateSiteRequest
{
    /// <summary>
    /// ID of the project this site belongs to
    /// </summary>
    [Required(ErrorMessage = "ProjectId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "ProjectId must be a positive number")]
    public int ProjectId { get; set; }

    /// <summary>
    /// Name of the site
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Geographic location of the site
    /// </summary>
    [StringLength(500, ErrorMessage = "Location cannot exceed 500 characters")]
    public string? Location { get; set; }
}

/// <summary>
/// Request DTO for updating a site
/// </summary>
public class UpdateSiteRequest
{
    /// <summary>
    /// Name of the site
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Geographic location of the site
    /// </summary>
    [StringLength(500, ErrorMessage = "Location cannot exceed 500 characters")]
    public string? Location { get; set; }
}

/// <summary>
/// Response DTO for site data
/// </summary>
public class SiteResponse
{
    /// <summary>
    /// ID of the site
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID of the project this site belongs to
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// Name of the project
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// Name of the site
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Geographic location of the site
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// When this site was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
