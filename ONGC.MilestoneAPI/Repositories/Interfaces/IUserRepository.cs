using ONGC.MilestoneAPI.Models.Entities;

namespace ONGC.MilestoneAPI.Repositories.Interfaces;

/// <summary>
/// Repository interface for User operations
/// </summary>
public interface IUserRepository : IGenericRepository<User>
{
    /// <summary>
    /// Get user by email
    /// </summary>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Check if email already exists
    /// </summary>
    Task<bool> EmailExistsAsync(string email);
}
