using ILChat.Entities.BaseEntities;
using Microsoft.EntityFrameworkCore;

namespace ILChat.Entities;

public class ChatDbContext(DbContextOptions<ChatDbContext> options, IHttpContextAccessor httpContextAccessor)
    : DbContext(options)
{
    public override int SaveChanges()
    {
        ApplyAuditInfo();
        return base.SaveChanges();
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditInfo()
    {
        var entries = ChangeTracker.Entries();
        var username = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

        foreach (var entry in entries)
        {
            if (entry.Entity is IAuditable auditable)
            {
                if (entry.State == EntityState.Added)
                {
                    auditable.CreatedAt = DateTime.UtcNow;
                    auditable.CreatedBy = username;
                }
                else if (entry.State == EntityState.Modified)
                {
                    auditable.UpdatedAt = DateTime.UtcNow;
                    auditable.UpdatedBy = username;
                }
            }

            if (entry.State == EntityState.Deleted && entry.Entity is IDeletable deletable)
            {
                entry.State = EntityState.Modified;
                deletable.IsDeleted = true;
                deletable.DeletedAt = DateTime.UtcNow;
                deletable.DeletedBy = username;
            }
        }
    }

    public DbSet<User> Users { get; set; } = null!;
}