namespace ONGC.MilestoneAPI.Models.DTOs;

/// <summary>
/// Response DTO for successful login
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// JWT authentication token
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Email of the authenticated user
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Role of the authenticated user
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Expiration time of the token
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}
