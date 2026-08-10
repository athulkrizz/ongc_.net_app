using Microsoft.IdentityModel.Tokens;
using ONGC.MilestoneAPI.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ONGC.MilestoneAPI.Helpers;

/// <summary>
/// Helper class for JWT token generation
/// </summary>
public static class JwtTokenHelper
{
    /// <summary>
    /// Generate a JWT token for a user
    /// </summary>
    /// <param name="user">The user to generate a token for</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>JWT token string</returns>
    public static string GenerateToken(User user, IConfiguration configuration)
    {
        var secret = configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
        var issuer = configuration["Jwt:Issuer"] ?? "ONGC.MilestoneAPI";
        var audience = configuration["Jwt:Audience"] ?? "ONGC.MilestoneAPI.Client";
        var expiryHours = double.Parse(configuration["Jwt:ExpiryHours"] ?? "24");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("UserId", user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expiryHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Get expiration time for a token
    /// </summary>
    /// <param name="configuration">Application configuration</param>
    /// <returns>Expiration DateTime</returns>
    public static DateTime GetTokenExpiration(IConfiguration configuration)
    {
        var expiryHours = double.Parse(configuration["Jwt:ExpiryHours"] ?? "24");
        return DateTime.UtcNow.AddHours(expiryHours);
    }
}
