using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TownHubPeople.Domain.Entities.Workforce;
using TownHubPeople.Domain.Interfaces;
using TownHubPeople.Web.Authorization;
using TownHubPeople.Web.ViewModels.Setup;

namespace TownHubPeople.Web.Controllers.Setup;

[AuthorizeTab("Setup", "Grades")]
[Route("Setup/[controller]")]
public class GradesController : MasterDataController<Grade, GradeViewModel>
{
    public GradesController(IUnitOfWork uow) : base(uow) { }
    protected override string ViewFolder => "~/Views/Setup/Grades";

    protected override IQueryable<Grade> Search(IQueryable<Grade> q, string t)
        => q.Where(x => x.Name.Contains(t) || x.Code.Contains(t));

    protected override IQueryable<Grade> Order(IQueryable<Grade> q, string? col, bool asc) => col switch
    {
        "code" => asc ? q.OrderBy(x => x.Code) : q.OrderByDescending(x => x.Code),
        "level" => asc ? q.OrderBy(x => x.Level) : q.OrderByDescending(x => x.Level),
        "isActive" => asc ? q.OrderBy(x => x.IsActive) : q.OrderByDescending(x => x.IsActive),
        _ => asc ? q.OrderBy(x => x.Name) : q.OrderByDescending(x => x.Name),
    };

    protected override object Project(Grade e) => new
    {
        id = e.Id,
        name = e.Name,
        code = e.Code,
        level = e.Level,
        isActive = e.IsActive
    };

    protected override GradeViewModel ToViewModel(Grade e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Code = e.Code,
        Level = e.Level,
        MinSalary = e.MinSalary,
        MaxSalary = e.MaxSalary,
        Description = e.Description,
        IsActive = e.IsActive
    };

    protected override void Apply(GradeViewModel vm, Grade e)
    {
        e.Name = vm.Name;
        e.Code = vm.Code;
        e.Level = vm.Level;
        e.MinSalary = vm.MinSalary;
        e.MaxSalary = vm.MaxSalary;
        e.Description = vm.Description;
        e.IsActive = vm.IsActive;
    }

    protected override async Task<string?> ValidateAsync(GradeViewModel vm, int id)
    {
        if (await Repo.QueryNoTracking().AnyAsync(x => x.Code == vm.Code && x.Id != id))
            return "A grade with this code already exists.";
        if (vm.MinSalary.HasValue && vm.MaxSalary.HasValue && vm.MaxSalary < vm.MinSalary)
            return "Max salary cannot be less than min salary.";
        return null;
    }
}
