namespace TownHubPeople.Application.Admin.MenuConfiguration;

public record TabItemDefinition(
    string ModuleKey,
    string TabKey,
    string DisplayName,
    string Icon,
    string IconColor,
    string Url,
    string PathSegment,
    int SortOrder,
    string? Group = null);

public static class SystemTabItems
{
    public static readonly List<TabItemDefinition> All =
    [
        // Workforce Setup
        new("Setup", "Departments", "Departments", "fa-building", "#0ea5e9", "/Setup/Departments", "/setup/departments", 1),
        new("Setup", "Designations", "Designations", "fa-id-badge", "#8b5cf6", "/Setup/Designations", "/setup/designations", 2),
        new("Setup", "Locations", "Locations", "fa-location-dot", "#10b981", "/Setup/Locations", "/setup/locations", 3),
        new("Setup", "Grades", "Grades", "fa-layer-group", "#f59e0b", "/Setup/Grades", "/setup/grades", 4),
        new("Setup", "CostCenters", "Cost Centers", "fa-coins", "#f43f5e", "/Setup/CostCenters", "/setup/costcenters", 5),
        new("Setup", "EmploymentTypes", "Employment Types", "fa-file-contract", "#0891b2", "/Setup/EmploymentTypes", "/setup/employmenttypes", 6),
        new("Setup", "Shifts", "Shifts", "fa-clock", "#ec4899", "/Setup/Shifts", "/setup/shifts", 7),
        new("Setup", "HolidayCalendars", "Holiday Calendars", "fa-calendar-day", "#fb923c", "/Setup/HolidayCalendars", "/setup/holidaycalendars", 8),
        new("Setup", "Skills", "Skills", "fa-lightbulb", "#22c55e", "/Setup/Skills", "/setup/skills", 9),

        // Administration
        new("Administration", "Users", "Users", "fa-users", "#0ea5e9", "/Admin/Users", "/admin/users", 1),
        new("Administration", "Roles", "Roles", "fa-user-shield", "#8b5cf6", "/Admin/Roles", "/admin/roles", 2),
        new("Administration", "AuditTrail", "Audit Trail", "fa-history", "#f59e0b", "/Admin/AuditTrail", "/admin/audittrail", 3),
        new("Administration", "MenuConfig", "Menu Config", "fa-bars", "#10b981", "/Admin/MenuConfig", "/admin/menuconfig", 4),
        new("Administration", "LoginSlides", "Login Slides", "fa-images", "#fb923c", "/Admin/LoginSlides", "/admin/loginslides", 5),
        new("Administration", "Organization", "Organization", "fa-building", "#0ea5e9", "/Admin/Organization", "/admin/organization", 6),
    ];

    public static List<TabItemDefinition> ForModule(string moduleKey)
        => All.Where(t => t.ModuleKey == moduleKey).OrderBy(t => t.SortOrder).ToList();
}
