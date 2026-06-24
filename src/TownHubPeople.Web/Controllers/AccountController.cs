using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TownHubPeople.Domain.Entities.Admin;
using TownHubPeople.Domain.Entities.Identity;
using TownHubPeople.Domain.Interfaces;
using TownHubPeople.Web.ViewModels.Account;
using Microsoft.EntityFrameworkCore;

namespace TownHubPeople.Web.Controllers;

public class AccountController(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    IUnitOfWork unitOfWork) : BaseController
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        ViewBag.Slides = await GetSlides(loginPage: true);
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(model);

        var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            return LocalRedirect(returnUrl ?? "/");
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Account locked out. Please try again later.");
            return View(model);
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Register()
    {
        ViewBag.Slides = await GetSlides(loginPage: false);
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            IsActive = true,
            EmailConfirmed = true,
            CreatedDate = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Viewer");
            await signInManager.SignInAsync(user, isPersistent: false);
            SuccessNotification("Registration successful! Welcome to TownHub People.");
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
            return RedirectToAction("Login");

        var roles = await userManager.GetRolesAsync(user);

        var model = new ProfileViewModel
        {
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            IsActive = user.IsActive,
            Roles = roles.ToList(),
            CreatedDate = user.CreatedDate,
            ProfileImageUrl = user.ProfileImageUrl
        };

        // Employee linkage is re-added when the HR domain is built.

        return View(model);
    }

    private async Task<List<LoginPageSlide>> GetSlides(bool loginPage)
    {
        return await unitOfWork.Repository<LoginPageSlide>()
            .QueryNoTracking()
            .Where(s => s.IsActive && (loginPage ? s.ShowOnLogin : s.ShowOnRegister))
            .OrderBy(s => s.SortOrder)
            .ToListAsync();
    }
}
