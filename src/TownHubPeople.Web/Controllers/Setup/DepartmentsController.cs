using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TownHubPeople.Domain.Entities.Workforce;
using TownHubPeople.Domain.Interfaces;
using TownHubPeople.Web.Authorization;
using TownHubPeople.Web.ViewModels.Setup;

namespace TownHubPeople.Web.Controllers.Setup;

[AuthorizeTab("Setup", "Departments")]
[Route("Setup/[controller]")]
public class DepartmentsController : MasterDataController<Department, DepartmentViewModel>
{
    public DepartmentsController(IUnitOfWork uow) : base(uow) { }
    protected override string ViewFolder => "~/Views/Setup/Departments";

    protected override IQueryable<Department> Includes(IQueryable<Department> q) => q.Include(x => x.ParentDepartment);

    protected override IQueryable<Department> Search(IQueryable<Department> q, string t)
        => q.Where(x => x.Name.Contains(t) || x.Code.Contains(t));

    protected override IQueryable<Department> Order(IQueryable<Department> q, string? col, bool asc) => col switch
    {
        "code" => asc ? q.OrderBy(x => x.Code) : q.OrderByDescending(x => x.Code),
        "isActive" => asc ? q.OrderBy(x => x.IsActive) : q.OrderByDescending(x => x.IsActive),
        _ => asc ? q.OrderBy(x => x.Name) : q.OrderByDescending(x => x.Name),
    };

    protected override object Project(Department e) => new
    {
        id = e.Id,
        name = e.Name,
        code = e.Code,
        parent = e.ParentDepartment != null ? e.ParentDepartment.Name : "",
        isActive = e.IsActive
    };

    protected override DepartmentViewModel ToViewModel(Department e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Code = e.Code,
        ParentDepartmentId = e.ParentDepartmentId,
        Description = e.Description,
        IsActive = e.IsActive
    };

    protected override void Apply(DepartmentViewModel vm, Department e)
    {
        e.Name = vm.Name;
        e.Code = vm.Code;
        e.ParentDepartmentId = vm.ParentDepartmentId;
        e.Description = vm.Description;
        e.IsActive = vm.IsActive;
    }

    protected override async Task PopulateLookupsAsync(DepartmentViewModel vm)
    {
        vm.AvailableParents = await Repo.QueryNoTracking()
            .Where(x => x.IsActive && x.Id != vm.Id)
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
            .ToListAsync();
    }

    protected override async Task<string?> ValidateAsync(DepartmentViewModel vm, int id)
        => await Repo.QueryNoTracking().AnyAsync(x => x.Code == vm.Code && x.Id != id)
            ? "A department with this code already exists."
            : null;
}
