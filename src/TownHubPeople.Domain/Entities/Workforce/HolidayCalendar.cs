using TownHubPeople.Domain.Common;

namespace TownHubPeople.Domain.Entities.Workforce;

public class HolidayCalendar : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public int Year { get; set; }

    public int? LocationId { get; set; }
    public Location? Location { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Holiday> Holidays { get; set; } = new List<Holiday>();
}
