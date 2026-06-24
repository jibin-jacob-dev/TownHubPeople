using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TownHubPeople.Domain.Common;
using TownHubPeople.Domain.Interfaces;

namespace TownHubPeople.Infrastructure.Data.Interceptors;

public class AuditTrailInterceptor(ICurrentUserService currentUser, IDateTimeService dateTime)
    : SaveChangesInterceptor
{
    private List<AuditEntry> _pendingAuditEntries = [];

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        _pendingAuditEntries = OnBeforeSaveChanges(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        OnAfterSaveChanges(eventData.Context);
        return base.SavedChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken ct = default)
    {
        _pendingAuditEntries = OnBeforeSaveChanges(eventData.Context);
        return base.SavingChangesAsync(eventData, result, ct);
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken ct = default)
    {
        await OnAfterSaveChangesAsync(eventData.Context, ct);
        return await base.SavedChangesAsync(eventData, result, ct);
    }

    private List<AuditEntry> OnBeforeSaveChanges(DbContext? context)
    {
        if (context is null) return [];

        context.ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditEntry>();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is AuditTrailEntry || entry.State is EntityState.Detached or EntityState.Unchanged)
                continue;

            var auditEntry = new AuditEntry
            {
                EntityName = entry.Entity.GetType().Name,
                UserId = currentUser.UserId ?? "System",
                UserName = currentUser.UserName ?? "System",
                Timestamp = dateTime.UtcNow,
                Action = entry.State switch
                {
                    EntityState.Added => "Created",
                    EntityState.Modified => "Modified",
                    EntityState.Deleted => "Deleted",
                    _ => "Unknown"
                }
            };

            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary)
                {
                    auditEntry.TemporaryProperties.Add(property);
                    continue;
                }

                string propertyName = property.Metadata.Name;

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                        break;
                    case EntityState.Deleted:
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        break;
                    case EntityState.Modified when property.IsModified && !Equals(property.OriginalValue, property.CurrentValue):
                        auditEntry.AffectedColumns.Add(propertyName);
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                        break;
                }
            }

            if (auditEntry.OldValues.Count > 0 || auditEntry.NewValues.Count > 0 || auditEntry.TemporaryProperties.Count > 0)
                auditEntries.Add(auditEntry);
        }

        foreach (var auditEntry in auditEntries.Where(e => !e.HasTemporaryProperties))
        {
            context.Set<AuditTrailEntry>().Add(auditEntry.ToAuditTrailEntry());
        }

        return auditEntries.Where(e => e.HasTemporaryProperties).ToList();
    }

    private void OnAfterSaveChanges(DbContext? context)
    {
        if (context is null || _pendingAuditEntries.Count == 0) return;

        foreach (var auditEntry in _pendingAuditEntries)
        {
            foreach (var prop in auditEntry.TemporaryProperties)
            {
                if (prop.Metadata.IsPrimaryKey())
                    auditEntry.EntityId = prop.CurrentValue?.ToString() ?? "";
                auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
            }
            context.Set<AuditTrailEntry>().Add(auditEntry.ToAuditTrailEntry());
        }
        context.SaveChanges();
        _pendingAuditEntries.Clear();
    }

    private async Task OnAfterSaveChangesAsync(DbContext? context, CancellationToken ct)
    {
        if (context is null || _pendingAuditEntries.Count == 0) return;

        foreach (var auditEntry in _pendingAuditEntries)
        {
            foreach (var prop in auditEntry.TemporaryProperties)
            {
                if (prop.Metadata.IsPrimaryKey())
                    auditEntry.EntityId = prop.CurrentValue?.ToString() ?? "";
                auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
            }
            context.Set<AuditTrailEntry>().Add(auditEntry.ToAuditTrailEntry());
        }
        await context.SaveChangesAsync(ct);
        _pendingAuditEntries.Clear();
    }
}

internal class AuditEntry
{
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public Dictionary<string, object?> OldValues { get; } = [];
    public Dictionary<string, object?> NewValues { get; } = [];
    public List<string> AffectedColumns { get; } = [];
    public List<PropertyEntry> TemporaryProperties { get; } = [];
    public bool HasTemporaryProperties => TemporaryProperties.Count > 0;

    public AuditTrailEntry ToAuditTrailEntry() => new()
    {
        EntityName = EntityName,
        EntityId = EntityId,
        UserId = UserId,
        UserName = UserName,
        Action = Action,
        Timestamp = Timestamp,
        OldValues = OldValues.Count > 0 ? JsonSerializer.Serialize(OldValues) : null,
        NewValues = NewValues.Count > 0 ? JsonSerializer.Serialize(NewValues) : null,
        AffectedColumns = AffectedColumns.Count > 0 ? JsonSerializer.Serialize(AffectedColumns) : null
    };
}
