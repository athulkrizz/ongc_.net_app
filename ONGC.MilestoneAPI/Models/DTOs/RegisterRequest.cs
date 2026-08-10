using System.ComponentModel.DataAnnotations;

namespace ONGC.MilestoneAPI.Models.DTOs;

/// <summary>
/// Request DTO for user registration
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// Email address for the new user
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Password for the new user
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Role for the user (optional, defaults to User)
    /// </summary>
    public string? Role { get; set; }
}
