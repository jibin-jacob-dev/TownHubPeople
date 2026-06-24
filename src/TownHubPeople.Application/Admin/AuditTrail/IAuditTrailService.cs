using TownHubPeople.Application.Common.Models;

namespace TownHubPeople.Application.Admin.AuditTrail;

public interface IAuditTrailService
{
    Task<DataTablesResponse<AuditTrailListDto>> GetDataTableAsync(
        DataTablesRequest request,
        string? entityFilter,
        string? actionFilter,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken ct = default);

    Task<AuditTrailDetailDto?> GetByIdAsync(long id, CancellationToken ct = default);

    Task<List<string>> GetDistinctEntityNamesAsync(CancellationToken ct = default);

    Task<List<AuditTrailDetailDto>> GetEntityHistoryAsync(
        string entityName, string entityId, CancellationToken ct = default);
}
