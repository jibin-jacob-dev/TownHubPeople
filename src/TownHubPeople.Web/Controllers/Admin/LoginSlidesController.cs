using TownHubPeople.Domain.Entities.Admin;
using TownHubPeople.Domain.Interfaces;
using TownHubPeople.Web.Authorization;
using TownHubPeople.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TownHubPeople.Web.Controllers.Admin;

[AuthorizeTab("Administration", "LoginSlides")]
[Route("Admin/[controller]")]
public class LoginSlidesController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    private const string ViewPath = "~/Views/Admin/LoginSlides";
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxImageSize = 5 * 1024 * 1024; // 5 MB

    public LoginSlidesController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }

    [Route("")]
    public IActionResult Index()
    {
        return View($"{ViewPath}/Index.cshtml");
    }

    [HttpPost]
    [Route("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var slides = await _unitOfWork.Repository<LoginPageSlide>()
            .QueryNoTracking()
            .OrderBy(x => x.SortOrder)
            .ToListAsync();

        return Json(new { data = slides });
    }

    [Route("Create")]
    public IActionResult Create()
    {
        return View($"{ViewPath}/Create.cshtml", new LoginSlideViewModel());
    }

    [HttpPost]
    [Route("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LoginSlideViewModel model, IFormFile? image)
    {
        if (!ModelState.IsValid)
            return View($"{ViewPath}/Create.cshtml", model);

        var entity = new LoginPageSlide
        {
            Title = model.Title,
            Description = model.Description,
            IconClass = model.IconClass,
            IconColor = model.IconColor,
            SortOrder = model.SortOrder,
            IsActive = model.IsActive,
            ShowOnLogin = model.ShowOnLogin,
            ShowOnRegister = model.ShowOnRegister
        };

        if (image is not null)
        {
            var (valid, errorMessage) = ValidateImage(image);
            if (!valid)
            {
                ModelState.AddModelError("image", errorMessage!);
                return View($"{ViewPath}/Create.cshtml", model);
            }

            entity.ImageFileName = await SaveImageAsync(image);
        }

        _unitOfWork.Repository<LoginPageSlide>().Add(entity);
        await _unitOfWork.SaveChangesAsync();

        SuccessNotification("Login slide created successfully.");
        return RedirectToAction(nameof(Index));
    }

    [Route("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _unitOfWork.Repository<LoginPageSlide>().GetByIdAsync(id);
        if (entity is null)
        {
            ErrorNotification("Login slide not found.");
            return RedirectToAction(nameof(Index));
        }

        var model = new LoginSlideViewModel
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            IconClass = entity.IconClass,
            IconColor = entity.IconColor,
            SortOrder = entity.SortOrder,
            IsActive = entity.IsActive,
            ShowOnLogin = entity.ShowOnLogin,
            ShowOnRegister = entity.ShowOnRegister,
            ExistingImageFileName = entity.ImageFileName
        };

        return View($"{ViewPath}/Edit.cshtml", model);
    }

    [HttpPost]
    [Route("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LoginSlideViewModel model, IFormFile? image)
    {
        if (!ModelState.IsValid)
            return View($"{ViewPath}/Edit.cshtml", model);

        var entity = await _unitOfWork.Repository<LoginPageSlide>().GetByIdAsync(id);
        if (entity is null)
        {
            ErrorNotification("Login slide not found.");
            return RedirectToAction(nameof(Index));
        }

        entity.Title = model.Title;
        entity.Description = model.Description;
        entity.IconClass = model.IconClass;
        entity.IconColor = model.IconColor;
        entity.SortOrder = model.SortOrder;
        entity.IsActive = model.IsActive;
        entity.ShowOnLogin = model.ShowOnLogin;
        entity.ShowOnRegister = model.ShowOnRegister;

        if (image is not null)
        {
            var (valid, errorMessage) = ValidateImage(image);
            if (!valid)
            {
                model.ExistingImageFileName = entity.ImageFileName;
                ModelState.AddModelError("image", errorMessage!);
                return View($"{ViewPath}/Edit.cshtml", model);
            }

            // Delete old image if it exists
            if (!string.IsNullOrEmpty(entity.ImageFileName))
                DeleteImage(entity.ImageFileName);

            entity.ImageFileName = await SaveImageAsync(image);
        }

        _unitOfWork.Repository<LoginPageSlide>().Update(entity);
        await _unitOfWork.SaveChangesAsync();

        SuccessNotification("Login slide updated successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Route("Delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _unitOfWork.Repository<LoginPageSlide>().GetByIdAsync(id);
        if (entity is null)
            return Json(new { success = false, message = "Login slide not found." });

        if (!string.IsNullOrEmpty(entity.ImageFileName))
            DeleteImage(entity.ImageFileName);

        _unitOfWork.Repository<LoginPageSlide>().Remove(entity);
        await _unitOfWork.SaveChangesAsync();

        return Json(new { success = true, message = "Record deleted successfully." });
    }

    [HttpPost]
    [Route("UpdateOrder")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateOrder([FromBody] List<SortOrderItem> items)
    {
        if (items is null || items.Count == 0)
            return Json(new { success = false, message = "No items provided." });

        foreach (var item in items)
        {
            var entity = await _unitOfWork.Repository<LoginPageSlide>().GetByIdAsync(item.Id);
            if (entity is not null)
            {
                entity.SortOrder = item.SortOrder;
                _unitOfWork.Repository<LoginPageSlide>().Update(entity);
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return Json(new { success = true });
    }

    // --- Private helpers ---

    private static (bool Valid, string? ErrorMessage) ValidateImage(IFormFile file)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            return (false, "Only JPG, PNG, and WebP images are allowed.");

        if (file.Length > MaxImageSize)
            return (false, "Image must be under 5 MB.");

        return (true, null);
    }

    private async Task<string> SaveImageAsync(IFormFile file)
    {
        var uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "slides");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsDir, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return fileName;
    }

    private void DeleteImage(string fileName)
    {
        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "slides", fileName);
        if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath);
    }

    public class SortOrderItem
    {
        public int Id { get; set; }
        public int SortOrder { get; set; }
    }
}
