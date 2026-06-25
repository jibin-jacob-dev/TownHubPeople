using TownHubPeople.Domain.Common;

namespace TownHubPeople.Domain.Entities.Workforce;

public class Designation : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public int? GradeId { get; set; }
    public Grade? Grade { get; set; }

    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}
