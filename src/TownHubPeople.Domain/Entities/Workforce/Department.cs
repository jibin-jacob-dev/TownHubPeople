using TownHubPeople.Domain.Common;

namespace TownHubPeople.Domain.Entities.Workforce;

public class Department : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    /// <summary>Optional parent for a department hierarchy (self-reference).</summary>
    public int? ParentDepartmentId { get; set; }
    public Department? ParentDepartment { get; set; }

    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}
