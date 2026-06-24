using TownHubPeople.Application.Admin.MenuConfiguration;
using TownHubPeople.Web.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TownHubPeople.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace TownHubPeople.Web.Controllers.Admin;

[AuthorizeTab("Administration", "MenuConfig")]
[Route("Admin/[controller]")]
public class MenuConfigController : BaseController
{
    private readonly IMenuConfigurationService _menuConfigService;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public MenuConfigController(IMenuConfigurationService menuConfigService, RoleManager<ApplicationRole> roleManager)
    {
        _menuConfigService = menuConfigService;
        _roleManager = roleManager;
    }

    [Route("")]
    public async Task<IActionResult> Index()
    {
        ViewBag.Roles   = await _roleManager.Roles.Select(r => r.Name!).OrderBy(r => r).ToListAsync();
        ViewBag.Modules = SystemMenuItems.All;
        ViewBag.AllTabs = SystemTabItems.All;
        return View("~/Views/Admin/MenuConfig/Index.cshtml");
    }

    [HttpGet, Route("GetRolePermissions")]
    public async Task<IActionResult> GetRolePermissions(string roleName)
    {
        var modules = SystemMenuItems.All.OrderBy(m => m.SortOrder).ToList();
        var result = new List<object>();
        foreach (var m in modules)
        {
            var hasMenuAccess = await _menuConfigService.HasMenuAccessAsync(roleName, m.Key);
            var grantedTabs   = await _menuConfigService.GetGrantedTabKeysAsync(roleName, m.Key);
            var allTabs       = SystemTabItems.ForModule(m.Key);
            var groups = allTabs
                .GroupBy(t => t.Group ?? "__flat__")
                .Select(g => new
                {
                    groupName = g.Key == "__flat__" ? (string?)null : g.Key,
                    tabs = g.Select(t => new { t.TabKey, t.DisplayName, t.Icon, t.IconColor, granted = grantedTabs.Contains(t.TabKey) }).ToList()
                }).ToList();
            result.Add(new { m.Key, moduleName = m.DisplayName, m.Icon, m.IconColor, hasMenuAccess, groups });
        }
        return Json(result);
    }

    [HttpPost, Route("ToggleMenuAccess"), ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleMenuAccess([FromBody] ToggleMenuRequest req)
    {
        if (req.Grant) await _menuConfigService.GrantMenuAccessAsync(req.RoleName, req.MenuKey);
        else           await _menuConfigService.RevokeMenuAccessAsync(req.RoleName, req.MenuKey);
        return Json(new { success = true });
    }

    [HttpPost, Route("ToggleTabAccess"), ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleTabAccess([FromBody] ToggleTabRequest req)
    {
        if (req.Grant) await _menuConfigService.GrantTabAccessAsync(req.RoleName, req.ModuleKey, req.TabKey);
        else           await _menuConfigService.RevokeTabAccessAsync(req.RoleName, req.ModuleKey, req.TabKey);
        return Json(new { success = true });
    }

    [HttpPost, Route("ToggleGroupAccess"), ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleGroupAccess([FromBody] ToggleGroupRequest req)
    {
        var tabs = SystemTabItems.All
            .Where(t => t.ModuleKey == req.ModuleKey && (t.Group == req.GroupName || (req.GroupName == null && t.Group == null)))
            .ToList();
        foreach (var tab in tabs)
        {
            if (req.Grant) await _menuConfigService.GrantTabAccessAsync(req.RoleName, req.ModuleKey, tab.TabKey);
            else           await _menuConfigService.RevokeTabAccessAsync(req.RoleName, req.ModuleKey, tab.TabKey);
        }
        var grantedTabKeys = await _menuConfigService.GetGrantedTabKeysAsync(req.RoleName, req.ModuleKey);
        return Json(new { success = true, grantedTabKeys });
    }

    [HttpGet, Route("GetTabsForModule")]
    public async Task<IActionResult> GetTabsForModule(string moduleKey)
    {
        var displayMap = await _menuConfigService.GetTabDisplayMapAsync(moduleKey);
        var tabs = SystemTabItems.ForModule(moduleKey)
            .Select(t =>
            {
                displayMap.TryGetValue(t.TabKey, out var cfg);
                return new
                {
                    t.TabKey,
                    displayName = cfg?.DisplayName ?? t.DisplayName,
                    icon        = cfg?.Icon ?? t.Icon,
                    iconColor   = cfg?.IconColor ?? t.IconColor,
                    sortOrder   = cfg?.SortOrder ?? t.SortOrder,
                    t.Group,
                    staticDisplayName = t.DisplayName,
                    staticIcon        = t.Icon,
                    staticIconColor   = t.IconColor,
                    isOverridden = cfg != null && (cfg.DisplayName != null || cfg.Icon != null || cfg.IconColor != null || cfg.SortOrder != null)
                };
            })
            .OrderBy(t => t.sortOrder)
            .ToList();
        return Json(tabs);
    }

    [HttpPost, Route("SaveTabDisplay"), ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveTabDisplay([FromBody] SaveTabDisplayRequest req)
    {
        await _menuConfigService.UpsertTabDisplayAsync(req.ModuleKey, req.TabKey, req.DisplayName, req.Icon, req.IconColor);
        return Json(new { success = true });
    }

    [HttpPost, Route("ResetTabDisplay"), ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetTabDisplay([FromBody] ResetTabDisplayRequest req)
    {
        await _menuConfigService.ResetTabDisplayAsync(req.ModuleKey, req.TabKey);
        return Json(new { success = true });
    }

    [HttpPost, Route("SaveTabOrder"), ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveTabOrder([FromBody] SaveTabOrderRequest req)
    {
        await _menuConfigService.SaveTabOrderAsync(req.ModuleKey, req.TabKeys);
        return Json(new { success = true });
    }

    [HttpPost, Route("SaveModuleOrder"), ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveModuleOrder([FromBody] SaveModuleOrderRequest req)
    {
        await _menuConfigService.SaveModuleOrderAsync(req.ModuleKeys);
        return Json(new { success = true });
    }

    [HttpPost, Route("CopyFromRole"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CopyFromRole([FromBody] CopyFromRoleRequest req)
    {
        if (string.IsNullOrEmpty(req.FromRole) || string.IsNullOrEmpty(req.ToRole))
            return Json(new { success = false, message = "Both roles are required." });

        await _menuConfigService.CopyPermissionsAsync(req.FromRole, req.ToRole);
        return Json(new { success = true });
    }
}

// Request DTOs (local to this file — no need for a separate file)
public record ToggleMenuRequest(string RoleName, string MenuKey, bool Grant);
public record ToggleTabRequest(string RoleName, string ModuleKey, string TabKey, bool Grant);
public record ToggleGroupRequest(string RoleName, string ModuleKey, string? GroupName, bool Grant);
public record SaveTabDisplayRequest(string ModuleKey, string TabKey, string? DisplayName, string? Icon, string? IconColor);
public record ResetTabDisplayRequest(string ModuleKey, string TabKey);
public record SaveTabOrderRequest(string ModuleKey, List<string> TabKeys);
public record SaveModuleOrderRequest(List<string> ModuleKeys);
public record CopyFromRoleRequest(string FromRole, string ToRole);
