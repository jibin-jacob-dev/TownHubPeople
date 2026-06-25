using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TownHubPeople.Domain.Entities.Workforce;
using TownHubPeople.Domain.Interfaces;
using TownHubPeople.Web.Authorization;
using TownHubPeople.Web.ViewModels.Setup;

namespace TownHubPeople.Web.Controllers.Setup;

[AuthorizeTab("Setup", "EmploymentTypes")]
[Route("Setup/[controller]")]
public class EmploymentTypesController : MasterDataController<EmploymentType, EmploymentTypeViewModel>
{
    public EmploymentTypesController(IUnitOfWork uow) : base(uow) { }
    protected override string ViewFolder => "~/Views/Setup/EmploymentTypes";

    protected override IQueryable<EmploymentType> Search(IQueryable<EmploymentType> q, string t)
        => q.Where(x => x.Name.Contains(t) || x.Code.Contains(t));

    protected override IQueryable<EmploymentType> Order(IQueryable<EmploymentType> q, string? col, bool asc) => col switch
    {
        "code" => asc ? q.OrderBy(x => x.Code) : q.OrderByDescending(x => x.Code),
        "isActive" => asc ? q.OrderBy(x => x.IsActive) : q.OrderByDescending(x => x.IsActive),
        _ => asc ? q.OrderBy(x => x.Name) : q.OrderByDescending(x => x.Name),
    };

    protected override object Project(EmploymentType e) => new { id = e.Id, name = e.Name, code = e.Code, isActive = e.IsActive };

    protected override EmploymentTypeViewModel ToViewModel(EmploymentType e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Code = e.Code,
        IsActive = e.IsActive
    };

    protected override void Apply(EmploymentTypeViewModel vm, EmploymentType e)
    {
        e.Name = vm.Name;
        e.Code = vm.Code;
        e.IsActive = vm.IsActive;
    }

    protected override async Task<string?> ValidateAsync(EmploymentTypeViewModel vm, int id)
        => await Repo.QueryNoTracking().AnyAsync(x => x.Code == vm.Code && x.Id != id)
            ? "An employment type with this code already exists."
            : null;
}
