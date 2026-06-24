using TownHubPeople.Application.Admin.MenuConfiguration;
using TownHubPeople.Domain.Entities.Admin;
using Microsoft.EntityFrameworkCore;

namespace TownHubPeople.Infrastructure.Data.Seeding;

public static class MenuConfigSeed
{
    // All modules Administrator must always have access to.
    private static readonly string[] AllMenuKeys =
        SystemMenuItems.All.Select(m => m.Key).ToArray();

    // Default module grants per role (additive — Administrator is handled separately).
    private static readonly Dictionary<string, string[]> DefaultMenuRoles = new()
    {
        ["Dashboard"]      = ["Administrator", "HRManager", "Recruiter", "PayrollManager", "LineManager", "Employee", "Viewer"],
        ["Administration"] = ["Administrator"],
    };

    // Default tab grants per module (Administrator handled separately via EnsureAdminFullAccessAsync).
    private static readonly Dictionary<string, string[]> DefaultTabRoles = new()
    {
        ["Administration"] = ["Administrator"],
    };

    /// <summary>Seeds default module visibility grants. Additive — never removes existing grants.</summary>
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var existing = await context.Set<RoleMenuAccess>()
            .Select(x => x.RoleName + "|" + x.MenuKey)
            .ToHashSetAsync();

        var toAdd = DefaultMenuRoles
            .SelectMany(kvp => kvp.Value.Select(role =>
                new RoleMenuAccess { MenuKey = kvp.Key, RoleName = role }))
            .Where(e => !existing.Contains(e.RoleName + "|" + e.MenuKey))
            .ToList();

        if (toAdd.Count > 0)
        {
            context.Set<RoleMenuAccess>().AddRange(toAdd);
            await context.SaveChangesAsync();
        }
    }

    /// <summary>Seeds default tab visibility grants. Additive — never removes existing grants.</summary>
    public static async Task SeedTabsAsync(ApplicationDbContext context)
    {
        var existing = await context.Set<RoleTabAccess>()
            .Select(x => x.RoleName + "|" + x.ModuleKey + "|" + x.TabKey)
            .ToHashSetAsync();

        var toAdd = SystemTabItems.All
            .Where(t => DefaultTabRoles.ContainsKey(t.ModuleKey))
            .SelectMany(t => DefaultTabRoles[t.ModuleKey]
                .Select(role => new RoleTabAccess { ModuleKey = t.ModuleKey, TabKey = t.TabKey, RoleName = role }))
            .Where(e => !existing.Contains(e.RoleName + "|" + e.ModuleKey + "|" + e.TabKey))
            .ToList();

        if (toAdd.Count > 0)
        {
            context.Set<RoleTabAccess>().AddRange(toAdd);
            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Ensures Administrator has access to EVERY module and tab defined in SystemTabItems/SystemMenuItems.
    /// Call this on every startup so new tabs/modules from future phases are auto-granted.
    /// </summary>
    public static async Task EnsureAdminFullAccessAsync(ApplicationDbContext context)
    {
        const string admin = "Administrator";

        // Modules
        var existingMenuKeys = await context.Set<RoleMenuAccess>()
            .Where(x => x.RoleName == admin)
            .Select(x => x.MenuKey)
            .ToHashSetAsync();

        var missingMenus = AllMenuKeys.Where(k => !existingMenuKeys.Contains(k))
            .Select(k => new RoleMenuAccess { RoleName = admin, MenuKey = k })
            .ToList();

        if (missingMenus.Count > 0)
            context.Set<RoleMenuAccess>().AddRange(missingMenus);

        // Tabs
        var existingTabKeys = await context.Set<RoleTabAccess>()
            .Where(x => x.RoleName == admin)
            .Select(x => x.ModuleKey + "|" + x.TabKey)
            .ToHashSetAsync();

        var missingTabs = SystemTabItems.All
            .Where(t => !existingTabKeys.Contains(t.ModuleKey + "|" + t.TabKey))
            .Select(t => new RoleTabAccess { RoleName = admin, ModuleKey = t.ModuleKey, TabKey = t.TabKey })
            .ToList();

        if (missingTabs.Count > 0)
            context.Set<RoleTabAccess>().AddRange(missingTabs);

        if (missingMenus.Count > 0 || missingTabs.Count > 0)
            await context.SaveChangesAsync();
    }
}
