using System.ComponentModel.DataAnnotations;

namespace ONGC.MilestoneAPI.Models.DTOs;

/// <summary>
/// Request DTO for user login
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Email address of the user
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Password of the user
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}
