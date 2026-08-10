using Microsoft.EntityFrameworkCore;
using ONGC.MilestoneAPI.Data;
using ONGC.MilestoneAPI.Models.Entities;
using ONGC.MilestoneAPI.Repositories.Interfaces;

namespace ONGC.MilestoneAPI.Repositories;

/// <summary>
/// Repository implementation for Milestone operations
/// </summary>
public class MilestoneRepository : GenericRepository<Milestone>, IMilestoneRepository
{
    public MilestoneRepository(AppDbContext context) : base(context)
    {
    }
}

