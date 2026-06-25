using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TownHubPeople.Domain.Entities.Workforce;
using TownHubPeople.Domain.Interfaces;
using TownHubPeople.Web.Authorization;
using TownHubPeople.Web.ViewModels.Setup;

namespace TownHubPeople.Web.Controllers.Setup;

[AuthorizeTab("Setup", "Designations")]
[Route("Setup/[controller]")]
public class DesignationsController : MasterDataController<Designation, DesignationViewModel>
{
    public DesignationsController(IUnitOfWork uow) : base(uow) { }
    protected override string ViewFolder => "~/Views/Setup/Designations";

    protected override IQueryable<Designation> Includes(IQueryable<Designation> q) => q.Include(x => x.Grade);

    protected override IQueryable<Designation> Search(IQueryable<Designation> q, string t)
        => q.Where(x => x.Name.Contains(t) || x.Code.Contains(t));

    protected override IQueryable<Designation> Order(IQueryable<Designation> q, string? col, bool asc) => col switch
    {
        "code" => asc ? q.OrderBy(x => x.Code) : q.OrderByDescending(x => x.Code),
        "isActive" => asc ? q.OrderBy(x => x.IsActive) : q.OrderByDescending(x => x.IsActive),
        _ => asc ? q.OrderBy(x => x.Name) : q.OrderByDescending(x => x.Name),
    };

    protected override object Project(Designation e) => new
    {
        id = e.Id,
        name = e.Name,
        code = e.Code,
        grade = e.Grade != null ? e.Grade.Name : "",
        isActive = e.IsActive
    };

    protected override DesignationViewModel ToViewModel(Designation e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Code = e.Code,
        GradeId = e.GradeId,
        Description = e.Description,
        IsActive = e.IsActive
    };

    protected override void Apply(DesignationViewModel vm, Designation e)
    {
        e.Name = vm.Name;
        e.Code = vm.Code;
        e.GradeId = vm.GradeId;
        e.Description = vm.Description;
        e.IsActive = vm.IsActive;
    }

    protected override async Task PopulateLookupsAsync(DesignationViewModel vm)
    {
        vm.AvailableGrades = await Uow.Repository<Grade>().QueryNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Level).ThenBy(x => x.Name)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
            .ToListAsync();
    }

    protected override async Task<string?> ValidateAsync(DesignationViewModel vm, int id)
        => await Repo.QueryNoTracking().AnyAsync(x => x.Code == vm.Code && x.Id != id)
            ? "A designation with this code already exists."
            : null;
}
