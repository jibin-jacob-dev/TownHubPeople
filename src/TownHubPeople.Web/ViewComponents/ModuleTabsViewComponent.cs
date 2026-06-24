using Microsoft.AspNetCore.Mvc;
using TownHubPeople.Application.Admin.MenuConfiguration;

namespace TownHubPeople.Web.ViewComponents;

public class ModuleTabsViewComponent(IMenuConfigurationService menuConfigService) : ViewComponent
{
    private static readonly Dictionary<string, string> GroupIcons = new()
    {
        ["People"]      = "fa-users",
        ["Leave"]       = "fa-calendar-minus",
        ["Payroll"]     = "fa-file-invoice-dollar",
        ["Time"]        = "fa-clock",
        ["Benefits"]    = "fa-hand-holding-heart",
        ["Recruitment"] = "fa-user-plus",
        ["Performance"] = "fa-chart-line",
        ["Learning"]    = "fa-graduation-cap",
        ["Reports"]     = "fa-chart-bar",
    };

    private static readonly Dictionary<string, string> GroupColors = new()
    {
        ["People"]      = "#0ea5e9",
        ["Leave"]       = "#10b981",
        ["Payroll"]     = "#f59e0b",
        ["Time"]        = "#8b5cf6",
        ["Benefits"]    = "#ef4444",
        ["Recruitment"] = "#3b82f6",
        ["Performance"] = "#f97316",
        ["Learning"]    = "#06b6d4",
        ["Reports"]     = "#6366f1",
    };

    public async Task<IViewComponentResult> InvokeAsync(string moduleKey)
    {
        var userRoles = HttpContext.User.Claims
            .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        var tabs = await menuConfigService.GetAccessibleTabsAsync(moduleKey, userRoles);

        // If no tab has a group, render the flat single-bar view.
        if (!tabs.Any(t => t.Group != null))
            return View(new ModuleTabsViewModel(false, tabs, []));

        var path = HttpContext.Request.Path.ToString().ToLowerInvariant();

        var groups = tabs
            .GroupBy(t => t.Group ?? t.DisplayName)
            .Select(g =>
            {
                var groupTabs = g.ToList();
                var isActive = groupTabs.Any(t => path.Contains(t.PathSegment));
                GroupIcons.TryGetValue(g.Key, out var icon);
                GroupColors.TryGetValue(g.Key, out var color);
                return new TabGroupViewModel(
                    Name: g.Key,
                    Icon: icon ?? "fa-circle-dot",
                    Color: color ?? "#64748b",
                    FirstTabUrl: groupTabs[0].Url,
                    IsActive: isActive,
                    Tabs: groupTabs);
            })
            .ToList();

        if (!groups.Any(g => g.IsActive))
            groups[0] = groups[0] with { IsActive = true };

        return View(new ModuleTabsViewModel(true, [], groups));
    }
}
