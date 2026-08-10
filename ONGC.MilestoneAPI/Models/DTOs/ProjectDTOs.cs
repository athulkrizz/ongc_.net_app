using System.ComponentModel.DataAnnotations;

namespace ONGC.MilestoneAPI.Models.DTOs;

/// <summary>
/// Request DTO for creating a project
/// </summary>
public class CreateProjectRequest
{
    /// <summary>
    /// ID of the company this project belongs to
    /// </summary>
    [Required(ErrorMessage = "CompanyId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "CompanyId must be a positive number")]
    public int CompanyId { get; set; }

    /// <summary>
    /// Name of the project
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO for updating a project
/// </summary>
public class UpdateProjectRequest
{
    /// <summary>
    /// Name of the project
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO for project data
/// </summary>
public class ProjectResponse
{
    /// <summary>
    /// ID of the project
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID of the company this project belongs to
    /// </summary>
    public int CompanyId { get; set; }

    /// <summary>
    /// Name of the company
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Name of the project
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// When this project was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
