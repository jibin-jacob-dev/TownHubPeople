using TownHubPeople.Domain.Common;

namespace TownHubPeople.Domain.Entities.Workforce;

public class Holiday : AuditableEntity
{
    public int HolidayCalendarId { get; set; }
    public HolidayCalendar? HolidayCalendar { get; set; }

    public DateOnly Date { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsOptional { get; set; }
}
