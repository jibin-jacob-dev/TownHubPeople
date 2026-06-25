using TownHubPeople.Domain.Common;

namespace TownHubPeople.Domain.Entities.Workforce;

public class EmploymentType : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
