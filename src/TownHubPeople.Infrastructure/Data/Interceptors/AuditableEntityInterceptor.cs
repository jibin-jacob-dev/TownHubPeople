using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TownHubPeople.Domain.Common;
using TownHubPeople.Domain.Interfaces;

namespace TownHubPeople.Infrastructure.Data.Interceptors;

public class AuditableEntityInterceptor(ICurrentUserService currentUser, IDateTimeService dateTime)
    : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken ct = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, ct);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context is null) return;

        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = currentUser.UserId ?? "System";
                entry.Entity.CreatedDate = dateTime.UtcNow;
            }

            if (entry.State is EntityState.Modified or EntityState.Added)
            {
                entry.Entity.LastModifiedBy = currentUser.UserId ?? "System";
                entry.Entity.LastModifiedDate = dateTime.UtcNow;
            }
        }
    }
}
