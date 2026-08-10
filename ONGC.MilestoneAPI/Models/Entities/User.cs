using System.ComponentModel.DataAnnotations;
using ONGC.MilestoneAPI.Models.Enums;

namespace ONGC.MilestoneAPI.Models.Entities;

/// <summary>
/// Represents a user in the system
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Email address of the user (used for login)
    /// </summary>
    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hashed password using BCrypt
    /// </summary>
    [Required]
    [StringLength(500)]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Role of the user in the system
    /// </summary>
    [Required]
    public UserRole Role { get; set; }
}
