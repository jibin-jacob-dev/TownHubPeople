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
