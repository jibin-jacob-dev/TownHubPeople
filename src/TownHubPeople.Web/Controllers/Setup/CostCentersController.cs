using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TownHubPeople.Domain.Entities.Workforce;
using TownHubPeople.Domain.Interfaces;
using TownHubPeople.Web.Authorization;
using TownHubPeople.Web.ViewModels.Setup;

namespace TownHubPeople.Web.Controllers.Setup;

[AuthorizeTab("Setup", "CostCenters")]
[Route("Setup/[controller]")]
public class CostCentersController : MasterDataController<CostCenter, CostCenterViewModel>
{
    public CostCentersController(IUnitOfWork uow) : base(uow) { }
    protected override string ViewFolder => "~/Views/Setup/CostCenters";

    protected override IQueryable<CostCenter> Search(IQueryable<CostCenter> q, string t)
        => q.Where(x => x.Name.Contains(t) || x.Code.Contains(t));

    protected override IQueryable<CostCenter> Order(IQueryable<CostCenter> q, string? col, bool asc) => col switch
    {
        "code" => asc ? q.OrderBy(x => x.Code) : q.OrderByDescending(x => x.Code),
        "isActive" => asc ? q.OrderBy(x => x.IsActive) : q.OrderByDescending(x => x.IsActive),
        _ => asc ? q.OrderBy(x => x.Name) : q.OrderByDescending(x => x.Name),
    };

    protected override object Project(CostCenter e) => new { id = e.Id, name = e.Name, code = e.Code, isActive = e.IsActive };

    protected override CostCenterViewModel ToViewModel(CostCenter e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Code = e.Code,
        Description = e.Description,
        IsActive = e.IsActive
    };

    protected override void Apply(CostCenterViewModel vm, CostCenter e)
    {
        e.Name = vm.Name;
        e.Code = vm.Code;
        e.Description = vm.Description;
        e.IsActive = vm.IsActive;
    }

    protected override async Task<string?> ValidateAsync(CostCenterViewModel vm, int id)
        => await Repo.QueryNoTracking().AnyAsync(x => x.Code == vm.Code && x.Id != id)
            ? "A cost center with this code already exists."
            : null;
}
