using TownHubPeople.Domain.Common;

namespace TownHubPeople.Domain.Entities.Workforce;

public class Grade : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    /// <summary>Ordering rank of the grade (1 = lowest band).</summary>
    public int Level { get; set; }

    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }

    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}
