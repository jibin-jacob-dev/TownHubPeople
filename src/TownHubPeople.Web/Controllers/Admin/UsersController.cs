using TownHubPeople.Application.Common.Models;
using TownHubPeople.Domain.Entities.Identity;
using TownHubPeople.Web.Authorization;
using TownHubPeople.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TownHubPeople.Web.Controllers.Admin;

[AuthorizeTab("Administration", "Users")]
[Route("Admin/[controller]")]
public class UsersController : BaseController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    private const string ViewPath = "~/Views/Admin/Users";

    public UsersController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
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
        var query = _userManager.Users.AsNoTracking();

        var searchValue = request.Search?.Value?.Trim();
        if (!string.IsNullOrEmpty(searchValue))
        {
            query = query.Where(x =>
                x.FirstName.Contains(searchValue)
                || x.LastName.Contains(searchValue)
                || x.Email!.Contains(searchValue));
        }

        var totalRecords = await _userManager.Users.CountAsync();
        var filteredRecords = await query.CountAsync();

        if (request.Order.Count > 0)
        {
            var order = request.Order[0];
            var columnName = request.Columns[order.Column].Data;
            query = columnName switch
            {
                "fullName" => order.Dir == "asc" ? query.OrderBy(x => x.LastName) : query.OrderByDescending(x => x.LastName),
                "email" => order.Dir == "asc" ? query.OrderBy(x => x.Email) : query.OrderByDescending(x => x.Email),
                "isActive" => order.Dir == "asc" ? query.OrderBy(x => x.IsActive) : query.OrderByDescending(x => x.IsActive),
                "createdDate" => order.Dir == "asc" ? query.OrderBy(x => x.CreatedDate) : query.OrderByDescending(x => x.CreatedDate),
                _ => query.OrderBy(x => x.LastName)
            };
        }
        else
        {
            query = query.OrderBy(x => x.LastName);
        }

        var users = await query
            .Skip(request.Start)
            .Take(request.Length)
            .ToListAsync();

        var data = new List<UserListItemDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            data.Add(new UserListItemDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Roles = string.Join(", ", roles),
                IsActive = user.IsActive,
                CreatedDate = user.CreatedDate
            });
        }

        return Json(new DataTablesResponse<UserListItemDto>
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
        var model = new CreateUserViewModel
        {
            AvailableRoles = await _roleManager.Roles.OrderBy(r => r.Name).Select(r => r.Name!).ToListAsync()
        };
        return View($"{ViewPath}/Create.cshtml", model);
    }

    [HttpPost]
    [Route("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        model.AvailableRoles = await _roleManager.Roles.OrderBy(r => r.Name).Select(r => r.Name!).ToListAsync();

        if (!ModelState.IsValid)
            return View($"{ViewPath}/Create.cshtml", model);

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            IsActive = model.IsActive,
            EmailConfirmed = true,
            CreatedDate = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View($"{ViewPath}/Create.cshtml", model);
        }

        if (model.SelectedRoles.Count > 0)
            await _userManager.AddToRolesAsync(user, model.SelectedRoles);

        SuccessNotification("User created successfully.");
        return RedirectToAction(nameof(Index));
    }

    [Route("Edit/{id}")]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            ErrorNotification("User not found.");
            return RedirectToAction(nameof(Index));
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        var model = new EditUserViewModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            IsActive = user.IsActive,
            SelectedRoles = userRoles.ToList(),
            AvailableRoles = await _roleManager.Roles.OrderBy(r => r.Name).Select(r => r.Name!).ToListAsync()
        };

        return View($"{ViewPath}/Edit.cshtml", model);
    }

    [HttpPost]
    [Route("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, EditUserViewModel model)
    {
        model.AvailableRoles = await _roleManager.Roles.OrderBy(r => r.Name).Select(r => r.Name!).ToListAsync();

        if (!ModelState.IsValid)
            return View($"{ViewPath}/Edit.cshtml", model);

        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            ErrorNotification("User not found.");
            return RedirectToAction(nameof(Index));
        }

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Email = model.Email;
        user.UserName = model.Email;
        user.IsActive = model.IsActive;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View($"{ViewPath}/Edit.cshtml", model);
        }

        if (!string.IsNullOrEmpty(model.NewPassword))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (!passwordResult.Succeeded)
            {
                foreach (var error in passwordResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View($"{ViewPath}/Edit.cshtml", model);
            }
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        var rolesToRemove = currentRoles.Except(model.SelectedRoles).ToList();
        var rolesToAdd = model.SelectedRoles.Except(currentRoles).ToList();

        if (rolesToRemove.Count > 0)
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
        if (rolesToAdd.Count > 0)
            await _userManager.AddToRolesAsync(user, rolesToAdd);

        SuccessNotification("User updated successfully.");
        return RedirectToAction(nameof(Index));
    }
}
