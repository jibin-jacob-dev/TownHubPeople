namespace TownHubPeople.Application.Admin.AuditTrail;

public class AuditTrailListDto
{
    public long Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class AuditTrailDetailDto
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public Dictionary<string, string?> OldValues { get; set; } = [];
    public Dictionary<string, string?> NewValues { get; set; } = [];
    public List<string> AffectedColumns { get; set; } = [];
}
