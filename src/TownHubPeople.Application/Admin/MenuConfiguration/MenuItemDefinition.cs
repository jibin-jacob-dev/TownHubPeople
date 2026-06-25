namespace TownHubPeople.Application.Admin.MenuConfiguration;

public record MenuItemDefinition(
    string Key,
    string DisplayName,
    string Icon,
    string Url,
    string IconColor,
    int SortOrder,
    string PathPrefix,
    string CssModuleClass);

public static class SystemMenuItems
{
    // HRMS modules. Only modules with real screens are listed today; People / Recruitment /
    // Payroll / Time / Performance / Learning / Documents slot in as those modules are built.
    public static readonly List<MenuItemDefinition> All =
    [
        new("Dashboard", "Dashboard", "fa-tachometer-alt", "/", "#60a5fa", 1, "/Home", "module-dashboard"),
        new("Setup", "Workforce Setup", "fa-sitemap", "/Setup/Departments", "#14b8a6", 2, "/Setup", "module-hr"),
        new("Administration", "Administration", "fa-shield-alt", "/Admin/Users", "#f43f5e", 9, "/Admin", "module-admin"),
    ];
}
