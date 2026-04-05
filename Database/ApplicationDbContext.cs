using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using project_basic.Interfaces;
using project_basic.Models;

namespace project_basic.Database;

public class ApplicationDbContext: DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor): base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var userIdStr = _httpContextAccessor?.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        Guid? userId = null;
        if (Guid.TryParse(userIdStr, out var parsed))
        {
            userId = parsed;
        }

        foreach (var entry in ChangeTracker.Entries())
        {
            var state = entry.State;

            // ===== AUDIT (BaseEntity) =====
            if (entry.Entity is BaseEntity baseEntity)
            {
                if (state == EntityState.Added)
                {
                    baseEntity.CreatedAt = now;
                    baseEntity.UpdatedAt = now;
                    baseEntity.CreatedBy = userId;
                    baseEntity.UpdatedBy = userId;
                }
                else if (state == EntityState.Modified)
                {
                    baseEntity.UpdatedAt = now;
                    baseEntity.UpdatedBy = userId;

                    // ❗ không cho sửa Created
                    entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                    entry.Property(nameof(BaseEntity.CreatedBy)).IsModified = false;
                }
            }

            // ===== SOFT DELETE =====
            if (entry.Entity is ISoftDelete softDelete)
            {
                if (state == EntityState.Deleted)
                {
                    // 👉 convert sang update
                    entry.State = EntityState.Modified;

                    softDelete.IsDeleted = true;
                    softDelete.DeletedAt = now;
                    softDelete.DeletedBy = userId;

                    // 👉 update audit nếu có
                    if (entry.Entity is BaseEntity b)
                    {
                        b.UpdatedAt = now;
                        b.UpdatedBy = userId;
                    }
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}