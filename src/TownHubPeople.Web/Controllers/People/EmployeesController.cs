using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TownHubPeople.Application.Common.Models;
using TownHubPeople.Domain.Entities.Identity;
using TownHubPeople.Domain.Entities.People;
using TownHubPeople.Domain.Entities.Workforce;
using TownHubPeople.Domain.Interfaces;
using TownHubPeople.Web.Authorization;
using TownHubPeople.Web.Controllers;
using TownHubPeople.Web.ViewModels.People;

namespace TownHubPeople.Web.Controllers.People;

[AuthorizeTab("People", "Employees")]
[Route("People/[controller]")]
public class EmployeesController : BaseController
{
    private readonly IUnitOfWork _uow;
    private readonly IDocumentNumberService _docNumbers;
    private readonly UserManager<ApplicationUser> _userManager;

    private const string ViewPath = "~/Views/People/Employees";

    public EmployeesController(IUnitOfWork uow, IDocumentNumberService docNumbers, UserManager<ApplicationUser> userManager)
    {
        _uow = uow;
        _docNumbers = docNumbers;
        _userManager = userManager;
    }

    private IRepository<Employee> Repo => _uow.Repository<Employee>();

    [Route("")]
    public IActionResult Index() => View($"{ViewPath}/Index.cshtml");

    [HttpPost]
    [Route("GetData")]
    public async Task<IActionResult> GetData([FromBody] DataTablesRequest request)
    {
        var total = await Repo.QueryNoTracking().CountAsync();

        var query = Repo.QueryNoTracking()
            .Include(e => e.Department)
            .Include(e => e.Designation)
            .Include(e => e.Manager)
            .AsQueryable();

        var term = request.Search?.Value?.Trim();
        if (!string.IsNullOrEmpty(term))
            query = query.Where(e =>
                e.FirstName.Contains(term) || e.LastName.Contains(term) ||
                e.EmployeeCode.Contains(term) || (e.WorkEmail != null && e.WorkEmail.Contains(term)));

        var filtered = await query.CountAsync();

        var asc = request.Order.Count == 0 || request.Order[0].Dir == "asc";
        var col = request.Order.Count > 0 ? request.Columns[request.Order[0].Column].Data : null;
        query = col switch
        {
            "employeeCode" => asc ? query.OrderBy(e => e.EmployeeCode) : query.OrderByDescending(e => e.EmployeeCode),
            "status" => asc ? query.OrderBy(e => e.Status) : query.OrderByDescending(e => e.Status),
            _ => asc ? query.OrderBy(e => e.FirstName).ThenBy(e => e.LastName)
                     : query.OrderByDescending(e => e.FirstName).ThenByDescending(e => e.LastName),
        };

        var rows = await query.Skip(request.Start).Take(request.Length).ToListAsync();

        return Json(new DataTablesResponse<object>
        {
            Draw = request.Draw,
            RecordsTotal = total,
            RecordsFiltered = filtered,
            Data = rows.Select(e => (object)new
            {
                id = e.Id,
                employeeCode = e.EmployeeCode,
                name = e.FullName,
                department = e.Department?.Name ?? "",
                designation = e.Designation?.Name ?? "",
                manager = e.Manager != null ? e.Manager.FullName : "",
                status = e.Status.ToString()
            }).ToList()
        });
    }

    [Route("Create")]
    public async Task<IActionResult> Create()
    {
        var vm = new EmployeeFormViewModel();
        await PopulateLookupsAsync(vm, null);
        return PartialView($"{ViewPath}/_Form.cshtml", vm);
    }

    [HttpPost]
    [Route("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeFormViewModel vm)
    {
        var error = ModelState.IsValid ? await ValidateAsync(vm, 0) : null;
        if (!ModelState.IsValid || error != null)
        {
            if (error != null) ModelState.AddModelError(string.Empty, error);
            return await Invalid(vm);
        }

        var entity = new Employee { EmployeeCode = await _docNumbers.GenerateNextNumberAsync("EMP") };
        Apply(vm, entity);
        Repo.Add(entity);
        await _uow.SaveChangesAsync();
        return Saved("Employee created successfully.");
    }

    [Route("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var e = await Repo.QueryNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (e is null) return RecordMissing("Employee not found.");

        var vm = new EmployeeFormViewModel
        {
            Id = e.Id,
            EmployeeCode = e.EmployeeCode,
            FirstName = e.FirstName,
            MiddleName = e.MiddleName,
            LastName = e.LastName,
            Gender = e.Gender,
            DateOfBirth = e.DateOfBirth,
            PersonalEmail = e.PersonalEmail,
            Phone = e.Phone,
            WorkEmail = e.WorkEmail,
            DepartmentId = e.DepartmentId,
            DesignationId = e.DesignationId,
            GradeId = e.GradeId,
            LocationId = e.LocationId,
            EmploymentTypeId = e.EmploymentTypeId,
            CostCenterId = e.CostCenterId,
            ManagerId = e.ManagerId,
            DateOfJoining = e.DateOfJoining,
            DateOfConfirmation = e.DateOfConfirmation,
            ProbationEndDate = e.ProbationEndDate,
            Status = e.Status,
            UserId = e.UserId
        };
        await PopulateLookupsAsync(vm, e.Id, e.UserId);
        return PartialView($"{ViewPath}/_Form.cshtml", vm);
    }

    [HttpPost]
    [Route("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EmployeeFormViewModel vm)
    {
        var error = ModelState.IsValid ? await ValidateAsync(vm, id) : null;
        if (!ModelState.IsValid || error != null)
        {
            if (error != null) ModelState.AddModelError(string.Empty, error);
            return await Invalid(vm);
        }

        var entity = await Repo.Query().FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return RecordMissing("Employee not found.");

        Apply(vm, entity);
        Repo.Update(entity);
        await _uow.SaveChangesAsync();
        return Saved("Employee updated successfully.");
    }

    [Route("Details/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var e = await Repo.QueryNoTracking()
            .Include(x => x.Department)
            .Include(x => x.Designation)
            .Include(x => x.Grade)
            .Include(x => x.Location)
            .Include(x => x.EmploymentType)
            .Include(x => x.CostCenter)
            .Include(x => x.Manager)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (e is null)
        {
            ErrorNotification("Employee not found.");
            return RedirectToAction(nameof(Index));
        }
        return View($"{ViewPath}/Details.cshtml", e);
    }

    [HttpPost]
    [Route("Delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await Repo.Query().FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return Json(new { success = false, message = "Employee not found." });

        if (await Repo.QueryNoTracking().AnyAsync(x => x.ManagerId == id))
            return Json(new { success = false, message = "Cannot delete — this employee is the reporting manager of others. Reassign their reports first." });

        Repo.Remove(entity);
        await _uow.SaveChangesAsync();
        return Json(new { success = true, message = "Record deleted successfully." });
    }

    // --- helpers ---
    private void Apply(EmployeeFormViewModel vm, Employee e)
    {
        e.FirstName = vm.FirstName;
        e.MiddleName = vm.MiddleName;
        e.LastName = vm.LastName;
        e.Gender = vm.Gender;
        e.DateOfBirth = vm.DateOfBirth;
        e.PersonalEmail = vm.PersonalEmail;
        e.Phone = vm.Phone;
        e.WorkEmail = vm.WorkEmail;
        e.DepartmentId = vm.DepartmentId;
        e.DesignationId = vm.DesignationId;
        e.GradeId = vm.GradeId;
        e.LocationId = vm.LocationId;
        e.EmploymentTypeId = vm.EmploymentTypeId;
        e.CostCenterId = vm.CostCenterId;
        e.ManagerId = vm.ManagerId == 0 ? null : vm.ManagerId;
        e.DateOfJoining = vm.DateOfJoining;
        e.DateOfConfirmation = vm.DateOfConfirmation;
        e.ProbationEndDate = vm.ProbationEndDate;
        e.Status = vm.Status;
        e.UserId = string.IsNullOrEmpty(vm.UserId) ? null : vm.UserId;
    }

    private async Task<string?> ValidateAsync(EmployeeFormViewModel vm, int id)
    {
        if (!string.IsNullOrWhiteSpace(vm.WorkEmail) &&
            await Repo.QueryNoTracking().AnyAsync(e => e.WorkEmail == vm.WorkEmail && e.Id != id))
            return "An employee with this work email already exists.";

        if (vm.ManagerId.HasValue && vm.ManagerId.Value == id)
            return "An employee cannot be their own manager.";

        return null;
    }

    private async Task PopulateLookupsAsync(EmployeeFormViewModel vm, int? excludeId, string? keepUserId = null)
    {
        vm.AvailableDepartments = await ActiveOptions<Department>(d => d.IsActive, d => d.Name);
        vm.AvailableDesignations = await ActiveOptions<Designation>(d => d.IsActive, d => d.Name);
        vm.AvailableGrades = await ActiveOptions<Grade>(g => g.IsActive, g => g.Name);
        vm.AvailableLocations = await ActiveOptions<Location>(l => l.IsActive, l => l.Name);
        vm.AvailableEmploymentTypes = await ActiveOptions<EmploymentType>(t => t.IsActive, t => t.Name);
        vm.AvailableCostCenters = await ActiveOptions<CostCenter>(c => c.IsActive, c => c.Name);

        var managers = await Repo.QueryNoTracking()
            .Where(e => e.Id != (excludeId ?? 0))
            .OrderBy(e => e.FirstName).ThenBy(e => e.LastName)
            .ToListAsync();
        vm.AvailableManagers = managers
            .Select(e => new SelectListItem { Value = e.Id.ToString(), Text = $"{e.FullName} ({e.EmployeeCode})" })
            .ToList();

        // Users not already linked to another employee (keep the currently-linked one on edit).
        var linkedUserIds = await Repo.QueryNoTracking()
            .Where(e => e.UserId != null && e.Id != (excludeId ?? 0))
            .Select(e => e.UserId!)
            .ToListAsync();
        var users = await _userManager.Users
            .Where(u => !linkedUserIds.Contains(u.Id))
            .OrderBy(u => u.Email)
            .ToListAsync();
        vm.AvailableUsers = users
            .Select(u => new SelectListItem { Value = u.Id, Text = $"{u.FullName} ({u.Email})" })
            .ToList();
    }

    private async Task<List<SelectListItem>> ActiveOptions<T>(
        System.Linq.Expressions.Expression<Func<T, bool>> active,
        Func<T, string> text) where T : Domain.Common.BaseEntity
    {
        var items = await _uow.Repository<T>().QueryNoTracking().Where(active).ToListAsync();
        return items
            .OrderBy(text)
            .Select(i => new SelectListItem { Value = i.Id.ToString(), Text = text(i) })
            .ToList();
    }

    private async Task<IActionResult> Invalid(EmployeeFormViewModel vm)
    {
        await PopulateLookupsAsync(vm, vm.Id, vm.UserId);
        Response.StatusCode = StatusCodes.Status400BadRequest;
        return PartialView($"{ViewPath}/_Form.cshtml", vm);
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
}
