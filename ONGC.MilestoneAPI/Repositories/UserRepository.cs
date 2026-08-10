using Microsoft.EntityFrameworkCore;
using ONGC.MilestoneAPI.Data;
using ONGC.MilestoneAPI.Models.Entities;
using ONGC.MilestoneAPI.Repositories.Interfaces;

namespace ONGC.MilestoneAPI.Repositories;

/// <summary>
/// Repository implementation for User operations
/// </summary>
public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email);
    }
}
