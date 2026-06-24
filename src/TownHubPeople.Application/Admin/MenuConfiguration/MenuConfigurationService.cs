using TownHubPeople.Domain.Entities.Admin;
using TownHubPeople.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TownHubPeople.Application.Admin.MenuConfiguration;

public class MenuConfigurationService(IUnitOfWork unitOfWork) : IMenuConfigurationService
{
    public async Task<List<MenuItemDefinition>> GetAccessibleMenuItemsAsync(IEnumerable<string> userRoles)
    {
        var roleList = userRoles.ToList();

        var accessibleKeys = await unitOfWork.Repository<RoleMenuAccess>()
            .QueryNoTracking()
            .Where(x => roleList.Contains(x.RoleName))
            .Select(x => x.MenuKey)
            .Distinct()
            .ToListAsync();

        var keySet = new HashSet<string>(accessibleKeys);

        var moduleMap = await GetModuleDisplayMapAsync();
        return SystemMenuItems.All
            .Where(m => keySet.Contains(m.Key))
            .Select(m =>
            {
                if (!moduleMap.TryGetValue(m.Key, out var cfg)) return m;
                return m with
                {
                    DisplayName = cfg.DisplayName ?? m.DisplayName,
                    SortOrder = cfg.SortOrder ?? m.SortOrder
                };
            })
            .OrderBy(m => m.SortOrder)
            .ToList();
    }

    public async Task<List<RoleMenuAccess>> GetAllAccessAsync(CancellationToken ct = default)
    {
        return await unitOfWork.Repository<RoleMenuAccess>()
            .QueryNoTracking()
            .OrderBy(x => x.MenuKey)
            .ThenBy(x => x.RoleName)
            .ToListAsync(ct);
    }

    public async Task SaveAccessAsync(List<(string MenuKey, string RoleName)> accessList, CancellationToken ct = default)
    {
        var repo = unitOfWork.Repository<RoleMenuAccess>();

        var existing = await repo.Query().ToListAsync(ct);

        var toRemove = existing
            .Where(e => !accessList.Any(a => a.MenuKey == e.MenuKey && a.RoleName == e.RoleName))
            .ToList();

        var toAdd = accessList
            .Where(a => !existing.Any(e => e.MenuKey == a.MenuKey && e.RoleName == a.RoleName))
            .Select(a => new RoleMenuAccess { MenuKey = a.MenuKey, RoleName = a.RoleName })
            .ToList();

        foreach (var item in toRemove)
            repo.Remove(item);

        foreach (var item in toAdd)
            repo.Add(item);

        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<List<TabItemDefinition>> GetAccessibleTabsAsync(string moduleKey, IEnumerable<string> userRoles)
    {
        var roleList = userRoles.ToList();

        var accessibleKeys = await unitOfWork.Repository<RoleTabAccess>()
            .QueryNoTracking()
            .Where(x => x.ModuleKey == moduleKey && roleList.Contains(x.RoleName))
            .Select(x => x.TabKey)
            .Distinct()
            .ToListAsync();

        var keySet = new HashSet<string>(accessibleKeys);

        var displayMap = await GetTabDisplayMapAsync(moduleKey);
        return SystemTabItems.ForModule(moduleKey)
            .Where(t => keySet.Contains(t.TabKey))
            .Select(t =>
            {
                if (!displayMap.TryGetValue(t.TabKey, out var cfg)) return t;
                return t with
                {
                    DisplayName = cfg.DisplayName ?? t.DisplayName,
                    Icon = cfg.Icon ?? t.Icon,
                    IconColor = cfg.IconColor ?? t.IconColor,
                    SortOrder = cfg.SortOrder ?? t.SortOrder
                };
            })
            .OrderBy(t => t.SortOrder)
            .ToList();
    }

    public async Task<List<RoleTabAccess>> GetAllTabAccessAsync(CancellationToken ct = default)
    {
        return await unitOfWork.Repository<RoleTabAccess>()
            .QueryNoTracking()
            .OrderBy(x => x.ModuleKey)
            .ThenBy(x => x.TabKey)
            .ThenBy(x => x.RoleName)
            .ToListAsync(ct);
    }

    public async Task SaveTabAccessAsync(List<(string ModuleKey, string TabKey, string RoleName)> accessList, CancellationToken ct = default)
    {
        var repo = unitOfWork.Repository<RoleTabAccess>();

        var existing = await repo.Query().ToListAsync(ct);

        var toRemove = existing
            .Where(e => !accessList.Any(a => a.ModuleKey == e.ModuleKey && a.TabKey == e.TabKey && a.RoleName == e.RoleName))
            .ToList();

        var toAdd = accessList
            .Where(a => !existing.Any(e => e.ModuleKey == a.ModuleKey && e.TabKey == a.TabKey && e.RoleName == a.RoleName))
            .Select(a => new RoleTabAccess { ModuleKey = a.ModuleKey, TabKey = a.TabKey, RoleName = a.RoleName })
            .ToList();

        foreach (var item in toRemove)
            repo.Remove(item);

        foreach (var item in toAdd)
            repo.Add(item);

        await unitOfWork.SaveChangesAsync(ct);
    }

    // ── Display config ────────────────────────────────────────────────────────

    public async Task<Dictionary<string, TabDisplayConfig>> GetTabDisplayMapAsync(string moduleKey, CancellationToken ct = default)
    {
        var configs = await unitOfWork.Repository<TabDisplayConfig>().QueryNoTracking()
            .Where(x => x.ModuleKey == moduleKey).ToListAsync(ct);
        return configs.ToDictionary(x => x.TabKey);
    }

    public async Task UpsertTabDisplayAsync(string moduleKey, string tabKey, string? displayName, string? icon, string? iconColor, CancellationToken ct = default)
    {
        var existing = await unitOfWork.Repository<TabDisplayConfig>().Query()
            .FirstOrDefaultAsync(x => x.ModuleKey == moduleKey && x.TabKey == tabKey, ct);
        if (existing is null)
        {
            unitOfWork.Repository<TabDisplayConfig>().Add(new TabDisplayConfig
            {
                ModuleKey = moduleKey, TabKey = tabKey,
                DisplayName = displayName, Icon = icon, IconColor = iconColor
            });
        }
        else
        {
            existing.DisplayName = displayName;
            existing.Icon = icon;
            existing.IconColor = iconColor;
            unitOfWork.Repository<TabDisplayConfig>().Update(existing);
        }
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task ResetTabDisplayAsync(string moduleKey, string tabKey, CancellationToken ct = default)
    {
        var existing = await unitOfWork.Repository<TabDisplayConfig>().Query()
            .FirstOrDefaultAsync(x => x.ModuleKey == moduleKey && x.TabKey == tabKey, ct);
        if (existing is not null)
        {
            unitOfWork.Repository<TabDisplayConfig>().Remove(existing);
            await unitOfWork.SaveChangesAsync(ct);
        }
    }

    public async Task<Dictionary<string, ModuleDisplayConfig>> GetModuleDisplayMapAsync(CancellationToken ct = default)
    {
        var configs = await unitOfWork.Repository<ModuleDisplayConfig>().QueryNoTracking().ToListAsync(ct);
        return configs.ToDictionary(x => x.ModuleKey);
    }

    public async Task SaveModuleOrderAsync(List<string> orderedModuleKeys, CancellationToken ct = default)
    {
        var existing = await unitOfWork.Repository<ModuleDisplayConfig>().Query().ToListAsync(ct);
        var existingMap = existing.ToDictionary(x => x.ModuleKey);
        for (int i = 0; i < orderedModuleKeys.Count; i++)
        {
            var key = orderedModuleKeys[i];
            if (existingMap.TryGetValue(key, out var cfg))
            {
                cfg.SortOrder = i + 1;
                unitOfWork.Repository<ModuleDisplayConfig>().Update(cfg);
            }
            else
            {
                unitOfWork.Repository<ModuleDisplayConfig>().Add(new ModuleDisplayConfig { ModuleKey = key, SortOrder = i + 1 });
            }
        }
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task SaveTabOrderAsync(string moduleKey, List<string> orderedTabKeys, CancellationToken ct = default)
    {
        var existing = await unitOfWork.Repository<TabDisplayConfig>().Query()
            .Where(x => x.ModuleKey == moduleKey).ToListAsync(ct);
        var existingMap = existing.ToDictionary(x => x.TabKey);
        for (int i = 0; i < orderedTabKeys.Count; i++)
        {
            var key = orderedTabKeys[i];
            if (existingMap.TryGetValue(key, out var cfg))
            {
                cfg.SortOrder = i + 1;
                unitOfWork.Repository<TabDisplayConfig>().Update(cfg);
            }
            else
            {
                unitOfWork.Repository<TabDisplayConfig>().Add(new TabDisplayConfig { ModuleKey = moduleKey, TabKey = key, SortOrder = i + 1 });
            }
        }
        await unitOfWork.SaveChangesAsync(ct);
    }

    // ── AJAX permission helpers ───────────────────────────────────────────────

    public async Task<bool> HasMenuAccessAsync(string roleName, string menuKey, CancellationToken ct = default)
        => await unitOfWork.Repository<RoleMenuAccess>().QueryNoTracking()
            .AnyAsync(x => x.RoleName == roleName && x.MenuKey == menuKey, ct);

    public async Task GrantMenuAccessAsync(string roleName, string menuKey, CancellationToken ct = default)
    {
        var exists = await HasMenuAccessAsync(roleName, menuKey, ct);
        if (!exists)
        {
            unitOfWork.Repository<RoleMenuAccess>().Add(new RoleMenuAccess { RoleName = roleName, MenuKey = menuKey });
            await unitOfWork.SaveChangesAsync(ct);
        }
    }

    public async Task RevokeMenuAccessAsync(string roleName, string menuKey, CancellationToken ct = default)
    {
        var existing = await unitOfWork.Repository<RoleMenuAccess>().Query()
            .FirstOrDefaultAsync(x => x.RoleName == roleName && x.MenuKey == menuKey, ct);
        if (existing is not null)
        {
            unitOfWork.Repository<RoleMenuAccess>().Remove(existing);
            await unitOfWork.SaveChangesAsync(ct);
        }
    }

    public async Task<HashSet<string>> GetGrantedTabKeysAsync(string roleName, string moduleKey, CancellationToken ct = default)
    {
        var keys = await unitOfWork.Repository<RoleTabAccess>().QueryNoTracking()
            .Where(x => x.RoleName == roleName && x.ModuleKey == moduleKey)
            .Select(x => x.TabKey).ToListAsync(ct);
        return [.. keys];
    }

    public async Task GrantTabAccessAsync(string roleName, string moduleKey, string tabKey, CancellationToken ct = default)
    {
        var exists = await unitOfWork.Repository<RoleTabAccess>().QueryNoTracking()
            .AnyAsync(x => x.RoleName == roleName && x.ModuleKey == moduleKey && x.TabKey == tabKey, ct);
        if (!exists)
        {
            unitOfWork.Repository<RoleTabAccess>().Add(new RoleTabAccess { RoleName = roleName, ModuleKey = moduleKey, TabKey = tabKey });
            await unitOfWork.SaveChangesAsync(ct);
        }
    }

    public async Task RevokeTabAccessAsync(string roleName, string moduleKey, string tabKey, CancellationToken ct = default)
    {
        var existing = await unitOfWork.Repository<RoleTabAccess>().Query()
            .FirstOrDefaultAsync(x => x.RoleName == roleName && x.ModuleKey == moduleKey && x.TabKey == tabKey, ct);
        if (existing is not null)
        {
            unitOfWork.Repository<RoleTabAccess>().Remove(existing);
            await unitOfWork.SaveChangesAsync(ct);
        }
    }

    public async Task CopyPermissionsAsync(string fromRole, string toRole, CancellationToken ct = default)
    {
        // Remove existing access for target role
        var existingMenus = await unitOfWork.Repository<RoleMenuAccess>().Query()
            .Where(x => x.RoleName == toRole).ToListAsync(ct);
        foreach (var m in existingMenus) unitOfWork.Repository<RoleMenuAccess>().Remove(m);

        var existingTabs = await unitOfWork.Repository<RoleTabAccess>().Query()
            .Where(x => x.RoleName == toRole).ToListAsync(ct);
        foreach (var t in existingTabs) unitOfWork.Repository<RoleTabAccess>().Remove(t);

        // Copy from source role
        var sourceMenus = await unitOfWork.Repository<RoleMenuAccess>().QueryNoTracking()
            .Where(x => x.RoleName == fromRole).ToListAsync(ct);
        foreach (var m in sourceMenus)
            unitOfWork.Repository<RoleMenuAccess>().Add(new RoleMenuAccess { RoleName = toRole, MenuKey = m.MenuKey });

        var sourceTabs = await unitOfWork.Repository<RoleTabAccess>().QueryNoTracking()
            .Where(x => x.RoleName == fromRole).ToListAsync(ct);
        foreach (var t in sourceTabs)
            unitOfWork.Repository<RoleTabAccess>().Add(new RoleTabAccess { RoleName = toRole, ModuleKey = t.ModuleKey, TabKey = t.TabKey });

        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task ApplyPresetAsync(string presetName, string toRole, CancellationToken ct = default)
    {
        // "Viewer" preset: access to all modules + all tabs
        if (presetName != "Viewer") return;

        var existingMenus = await unitOfWork.Repository<RoleMenuAccess>().Query()
            .Where(x => x.RoleName == toRole).ToListAsync(ct);
        foreach (var m in existingMenus) unitOfWork.Repository<RoleMenuAccess>().Remove(m);

        var existingTabs = await unitOfWork.Repository<RoleTabAccess>().Query()
            .Where(x => x.RoleName == toRole).ToListAsync(ct);
        foreach (var t in existingTabs) unitOfWork.Repository<RoleTabAccess>().Remove(t);

        foreach (var menu in SystemMenuItems.All)
            unitOfWork.Repository<RoleMenuAccess>().Add(new RoleMenuAccess { RoleName = toRole, MenuKey = menu.Key });

        foreach (var tab in SystemTabItems.All)
            unitOfWork.Repository<RoleTabAccess>().Add(new RoleTabAccess { RoleName = toRole, ModuleKey = tab.ModuleKey, TabKey = tab.TabKey });

        await unitOfWork.SaveChangesAsync(ct);
    }
}
