using System.ComponentModel.DataAnnotations;

namespace ONGC.MilestoneAPI.Models.DTOs;

/// <summary>
/// Request DTO for creating a company
/// </summary>
public class CreateCompanyRequest
{
    /// <summary>
    /// Name of the company
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO for updating a company
/// </summary>
public class UpdateCompanyRequest
{
    /// <summary>
    /// Name of the company
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO for company data
/// </summary>
public class CompanyResponse
{
    /// <summary>
    /// ID of the company
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the company
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// When this company was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
