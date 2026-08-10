using Microsoft.EntityFrameworkCore;
using ONGC.MilestoneAPI.Models.Entities;

namespace ONGC.MilestoneAPI.Data;

/// <summary>
/// Simplified database context for the ONGC Milestone API
/// </summary>
public class AppDbContext : DbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor? httpContextAccessor = null)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Milestones in the well construction process
    /// </summary>
    public DbSet<Milestone> Milestones { get; set; }

    /// <summary>
    /// Users of the system
    /// </summary>
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Milestone configuration
        modelBuilder.Entity<Milestone>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Asset);
            entity.HasIndex(e => e.Well);
            entity.HasIndex(e => e.Wellbore);
            entity.HasIndex(e => e.Status);
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
        });
    }

    /// <summary>
    /// Override SaveChangesAsync to automatically populate audit fields
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        var currentUserId = GetCurrentUserId();
        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
                entry.Entity.CreatedBy = currentUserId;
                entry.Entity.UpdatedBy = currentUserId;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedBy = currentUserId;
                // Prevent overwriting CreatedAt and CreatedBy
                entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                entry.Property(nameof(BaseEntity.CreatedBy)).IsModified = false;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Get the current user ID from the HTTP context
    /// </summary>
    private int? GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor?.HttpContext?.User?.FindFirst("UserId");
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return null;
    }
}
