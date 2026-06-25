using TownHubPeople.Domain.Common;

namespace TownHubPeople.Domain.Entities.Workforce;

public class Skill : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}
