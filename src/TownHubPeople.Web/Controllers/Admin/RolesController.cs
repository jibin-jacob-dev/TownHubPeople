using TownHubPeople.Application.Admin.MenuConfiguration;
using TownHubPeople.Application.Common.Models;
using TownHubPeople.Domain.Entities.Identity;
using TownHubPeople.Web.Authorization;
using TownHubPeople.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TownHubPeople.Web.Controllers.Admin;

[AuthorizeTab("Administration", "Roles")]
[Route("Admin/[controller]")]
public class RolesController : BaseController
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMenuConfigurationService _menuConfigService;

    private const string ViewPath = "~/Views/Admin/Roles";

    // Built-in presets available on role creation
    public static readonly string[] AvailablePresets = ["Viewer"];

    public RolesController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, IMenuConfigurationService menuConfigService)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _menuConfigService = menuConfigService;
    }

    // AJAX-aware result helpers (see UsersController for the convention). InvalidCreate
    // re-populates the dropdown data the create partial needs.
    private async Task<IActionResult> InvalidCreate(CreateRoleViewModel model)
    {
        ViewBag.ExistingRoles = await _roleManager.Roles.Select(r => r.Name!).OrderBy(r => r).ToListAsync();
        ViewBag.AvailablePresets = AvailablePresets;
        if (IsAjax()) { Response.StatusCode = 400; return PartialView($"{ViewPath}/_CreateForm.cshtml", model); }
        return View($"{ViewPath}/Create.cshtml", model);
    }

    private IActionResult InvalidEdit(string id, CreateRoleViewModel model)
    {
        ViewBag.Id = id;
        if (IsAjax()) { Response.StatusCode = 400; return PartialView($"{ViewPath}/_EditForm.cshtml", model); }
        return View($"{ViewPath}/Edit.cshtml", model);
    }

    private IActionResult Saved(string message)
    {
        if (IsAjax()) return Json(new { success = true, message });
        SuccessNotification(message);
        return RedirectToAction(nameof(Index));
    }

    private IActionResult RecordMissing(string message)
    {
        if (IsAjax()) return Json(new { success = false, message });
        ErrorNotification(message);
        return RedirectToAction(nameof(Index));
    }

    [Route("")]
    public IActionResult Index()
    {
        return View($"{ViewPath}/Index.cshtml");
    }

    [HttpPost]
    [Route("GetData")]
    public async Task<IActionResult> GetData([FromBody] DataTablesRequest request)
    {
        var query = _roleManager.Roles.AsNoTracking();

        var searchValue = request.Search?.Value?.Trim();
        if (!string.IsNullOrEmpty(searchValue))
        {
            query = query.Where(x =>
                x.Name!.Contains(searchValue)
                || (x.Description != null && x.Description.Contains(searchValue)));
        }

        var totalRecords = await _roleManager.Roles.CountAsync();
        var filteredRecords = await query.CountAsync();

        if (request.Order.Count > 0)
        {
            var order = request.Order[0];
            var columnName = request.Columns[order.Column].Data;
            query = columnName switch
            {
                "name" => order.Dir == "asc" ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name),
                "description" => order.Dir == "asc" ? query.OrderBy(x => x.Description) : query.OrderByDescending(x => x.Description),
                "createdDate" => order.Dir == "asc" ? query.OrderBy(x => x.CreatedDate) : query.OrderByDescending(x => x.CreatedDate),
                _ => query.OrderBy(x => x.Name)
            };
        }
        else
        {
            query = query.OrderBy(x => x.Name);
        }

        var roles = await query
            .Skip(request.Start)
            .Take(request.Length)
            .ToListAsync();

        var data = new List<RoleListItemDto>();
        foreach (var role in roles)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
            data.Add(new RoleListItemDto
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty,
                Description = role.Description,
                UserCount = usersInRole.Count,
                CreatedDate = role.CreatedDate
            });
        }

        return Json(new DataTablesResponse<RoleListItemDto>
        {
            Draw = request.Draw,
            RecordsTotal = totalRecords,
            RecordsFiltered = filteredRecords,
            Data = data
        });
    }

    [Route("Create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.ExistingRoles = await _roleManager.Roles.Select(r => r.Name!).OrderBy(r => r).ToListAsync();
        ViewBag.AvailablePresets = AvailablePresets;
        if (IsAjax())
            return PartialView($"{ViewPath}/_CreateForm.cshtml", new CreateRoleViewModel());
        return View($"{ViewPath}/Create.cshtml", new CreateRoleViewModel());
    }

    [HttpPost]
    [Route("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRoleViewModel model)
    {
        if (!ModelState.IsValid)
            return await InvalidCreate(model);

        var exists = await _roleManager.RoleExistsAsync(model.Name);
        if (exists)
        {
            ModelState.AddModelError(nameof(model.Name), "A role with this name already exists.");
            return await InvalidCreate(model);
        }

        var role = new ApplicationRole
        {
            Name = model.Name,
            Description = model.Description,
            CreatedDate = DateTime.UtcNow
        };

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return await InvalidCreate(model);
        }

        // Apply permissions from source role or preset
        if (!string.IsNullOrEmpty(model.CopyFromRole))
            await _menuConfigService.CopyPermissionsAsync(model.CopyFromRole, model.Name);
        else if (!string.IsNullOrEmpty(model.ApplyPreset))
            await _menuConfigService.ApplyPresetAsync(model.ApplyPreset, model.Name);

        var msg = !string.IsNullOrEmpty(model.CopyFromRole)
            ? $"Role created and permissions copied from \"{model.CopyFromRole}\"."
            : !string.IsNullOrEmpty(model.ApplyPreset)
                ? $"Role created with \"{model.ApplyPreset}\" preset applied."
                : "Role created. Use Menu Config to assign access.";

        return Saved(msg);
    }

    [Route("Edit/{id}")]
    public async Task<IActionResult> Edit(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role is null)
            return RecordMissing("Role not found.");

        var model = new CreateRoleViewModel
        {
            Name = role.Name ?? string.Empty,
            Description = role.Description
        };

        ViewBag.Id = id;
        if (IsAjax())
            return PartialView($"{ViewPath}/_EditForm.cshtml", model);
        return View($"{ViewPath}/Edit.cshtml", model);
    }

    [HttpPost]
    [Route("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, CreateRoleViewModel model)
    {
        if (!ModelState.IsValid)
            return InvalidEdit(id, model);

        var role = await _roleManager.FindByIdAsync(id);
        if (role is null)
            return RecordMissing("Role not found.");

        var duplicate = await _roleManager.Roles.AnyAsync(r => r.Name == model.Name && r.Id != id);
        if (duplicate)
        {
            ModelState.AddModelError(nameof(model.Name), "A role with this name already exists.");
            return InvalidEdit(id, model);
        }

        role.Name = model.Name;
        role.Description = model.Description;

        var result = await _roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return InvalidEdit(id, model);
        }

        return Saved("Role updated successfully.");
    }

    [HttpPost]
    [Route("Delete/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role is null)
            return Json(new { success = false, message = "Role not found." });

        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
        if (usersInRole.Count > 0)
            return Json(new { success = false, message = $"Cannot delete — {usersInRole.Count} user(s) are assigned to this role." });

        var result = await _roleManager.DeleteAsync(role);
        return Json(new { success = result.Succeeded, message = result.Succeeded ? "Record deleted successfully." : string.Join(", ", result.Errors.Select(e => e.Description)) });
    }
}
