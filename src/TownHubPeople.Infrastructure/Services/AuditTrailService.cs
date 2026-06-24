using System.Text.Json;
using TownHubPeople.Application.Admin.AuditTrail;
using TownHubPeople.Application.Common.Models;
using TownHubPeople.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TownHubPeople.Infrastructure.Services;

public class AuditTrailService(ApplicationDbContext context) : IAuditTrailService
{
    public async Task<DataTablesResponse<AuditTrailListDto>> GetDataTableAsync(
        DataTablesRequest request,
        string? entityFilter,
        string? actionFilter,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken ct = default)
    {
        var query = context.AuditTrailEntries.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(entityFilter))
            query = query.Where(x => x.EntityName == entityFilter);

        if (!string.IsNullOrEmpty(actionFilter))
            query = query.Where(x => x.Action == actionFilter);

        if (fromDate.HasValue)
            query = query.Where(x => x.Timestamp >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(x => x.Timestamp < toDate.Value.AddDays(1));

        var searchValue = request.Search?.Value?.Trim();
        if (!string.IsNullOrEmpty(searchValue))
        {
            query = query.Where(x =>
                x.UserName.Contains(searchValue)
                || x.EntityName.Contains(searchValue)
                || x.EntityId.Contains(searchValue));
        }

        var totalRecords = await context.AuditTrailEntries.CountAsync(ct);
        var filteredRecords = await query.CountAsync(ct);

        if (request.Order.Count > 0)
        {
            var order = request.Order[0];
            var columnName = request.Columns[order.Column].Data;
            query = columnName switch
            {
                "userName" => order.Dir == "asc" ? query.OrderBy(x => x.UserName) : query.OrderByDescending(x => x.UserName),
                "entityName" => order.Dir == "asc" ? query.OrderBy(x => x.EntityName) : query.OrderByDescending(x => x.EntityName),
                "action" => order.Dir == "asc" ? query.OrderBy(x => x.Action) : query.OrderByDescending(x => x.Action),
                "timestamp" => order.Dir == "asc" ? query.OrderBy(x => x.Timestamp) : query.OrderByDescending(x => x.Timestamp),
                _ => query.OrderByDescending(x => x.Timestamp)
            };
        }
        else
        {
            query = query.OrderByDescending(x => x.Timestamp);
        }

        var data = await query
            .Skip(request.Start)
            .Take(request.Length)
            .Select(x => new AuditTrailListDto
            {
                Id = x.Id,
                UserName = x.UserName,
                EntityName = x.EntityName,
                EntityId = x.EntityId,
                Action = x.Action,
                Timestamp = x.Timestamp
            })
            .ToListAsync(ct);

        return new DataTablesResponse<AuditTrailListDto>
        {
            Draw = request.Draw,
            RecordsTotal = totalRecords,
            RecordsFiltered = filteredRecords,
            Data = data
        };
    }

    public async Task<AuditTrailDetailDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var entry = await context.AuditTrailEntries.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (entry is null)
            return null;

        return new AuditTrailDetailDto
        {
            Id = entry.Id,
            UserId = entry.UserId,
            UserName = entry.UserName,
            EntityName = entry.EntityName,
            EntityId = entry.EntityId,
            Action = entry.Action,
            Timestamp = entry.Timestamp,
            OldValues = DeserializeJson(entry.OldValues),
            NewValues = DeserializeJson(entry.NewValues),
            AffectedColumns = DeserializeList(entry.AffectedColumns)
        };
    }

    public async Task<List<string>> GetDistinctEntityNamesAsync(CancellationToken ct = default)
    {
        return await context.AuditTrailEntries.AsNoTracking()
            .Select(x => x.EntityName)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(ct);
    }

    public async Task<List<AuditTrailDetailDto>> GetEntityHistoryAsync(
        string entityName, string entityId, CancellationToken ct = default)
    {
        var entries = await context.AuditTrailEntries.AsNoTracking()
            .Where(e => e.EntityName == entityName && e.EntityId == entityId)
            .OrderByDescending(e => e.Timestamp)
            .Take(100)
            .ToListAsync(ct);

        return entries.Select(e => new AuditTrailDetailDto
        {
            Id = e.Id,
            UserId = e.UserId,
            UserName = e.UserName,
            EntityName = e.EntityName,
            EntityId = e.EntityId,
            Action = e.Action,
            Timestamp = e.Timestamp,
            OldValues = DeserializeJson(e.OldValues),
            NewValues = DeserializeJson(e.NewValues),
            AffectedColumns = DeserializeList(e.AffectedColumns)
        }).ToList();
    }

    private static Dictionary<string, string?> DeserializeJson(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return [];

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)?
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ValueKind == JsonValueKind.Null ? null : kvp.Value.ToString())
                ?? [];
        }
        catch
        {
            return [];
        }
    }

    private static List<string> DeserializeList(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return [];

        try
        {
            return JsonSerializer.Deserialize<List<string>>(json) ?? [];
        }
        catch
        {
            return [];
        }
    }
}
