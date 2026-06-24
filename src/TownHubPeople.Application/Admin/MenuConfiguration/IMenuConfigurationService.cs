using TownHubPeople.Domain.Entities.Admin;

namespace TownHubPeople.Application.Admin.MenuConfiguration;

public interface IMenuConfigurationService
{
    Task<List<MenuItemDefinition>> GetAccessibleMenuItemsAsync(IEnumerable<string> userRoles);
    Task<List<RoleMenuAccess>> GetAllAccessAsync(CancellationToken ct = default);
    Task SaveAccessAsync(List<(string MenuKey, string RoleName)> accessList, CancellationToken ct = default);

    Task<List<TabItemDefinition>> GetAccessibleTabsAsync(string moduleKey, IEnumerable<string> userRoles);
    Task<List<RoleTabAccess>> GetAllTabAccessAsync(CancellationToken ct = default);
    Task SaveTabAccessAsync(List<(string ModuleKey, string TabKey, string RoleName)> accessList, CancellationToken ct = default);

    // Display config
    Task<Dictionary<string, TabDisplayConfig>> GetTabDisplayMapAsync(string moduleKey, CancellationToken ct = default);
    Task UpsertTabDisplayAsync(string moduleKey, string tabKey, string? displayName, string? icon, string? iconColor, CancellationToken ct = default);
    Task ResetTabDisplayAsync(string moduleKey, string tabKey, CancellationToken ct = default);
    Task<Dictionary<string, ModuleDisplayConfig>> GetModuleDisplayMapAsync(CancellationToken ct = default);
    Task SaveModuleOrderAsync(List<string> orderedModuleKeys, CancellationToken ct = default);
    Task SaveTabOrderAsync(string moduleKey, List<string> orderedTabKeys, CancellationToken ct = default);

    // Permission copy
    Task CopyPermissionsAsync(string fromRole, string toRole, CancellationToken ct = default);
    Task ApplyPresetAsync(string presetName, string toRole, CancellationToken ct = default);

    // AJAX permission helpers
    Task<bool> HasMenuAccessAsync(string roleName, string menuKey, CancellationToken ct = default);
    Task GrantMenuAccessAsync(string roleName, string menuKey, CancellationToken ct = default);
    Task RevokeMenuAccessAsync(string roleName, string menuKey, CancellationToken ct = default);
    Task<HashSet<string>> GetGrantedTabKeysAsync(string roleName, string moduleKey, CancellationToken ct = default);
    Task GrantTabAccessAsync(string roleName, string moduleKey, string tabKey, CancellationToken ct = default);
    Task RevokeTabAccessAsync(string roleName, string moduleKey, string tabKey, CancellationToken ct = default);
}
