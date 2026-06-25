using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TownHubPeople.Domain.Entities.Workforce;
using TownHubPeople.Domain.Interfaces;
using TownHubPeople.Web.Authorization;
using TownHubPeople.Web.ViewModels.Setup;

namespace TownHubPeople.Web.Controllers.Setup;

[AuthorizeTab("Setup", "Locations")]
[Route("Setup/[controller]")]
public class LocationsController : MasterDataController<Location, LocationViewModel>
{
    public LocationsController(IUnitOfWork uow) : base(uow) { }
    protected override string ViewFolder => "~/Views/Setup/Locations";

    protected override IQueryable<Location> Search(IQueryable<Location> q, string t)
        => q.Where(x => x.Name.Contains(t) || x.Code.Contains(t) || (x.City != null && x.City.Contains(t)));

    protected override IQueryable<Location> Order(IQueryable<Location> q, string? col, bool asc) => col switch
    {
        "code" => asc ? q.OrderBy(x => x.Code) : q.OrderByDescending(x => x.Code),
        "city" => asc ? q.OrderBy(x => x.City) : q.OrderByDescending(x => x.City),
        "isActive" => asc ? q.OrderBy(x => x.IsActive) : q.OrderByDescending(x => x.IsActive),
        _ => asc ? q.OrderBy(x => x.Name) : q.OrderByDescending(x => x.Name),
    };

    protected override object Project(Location e) => new
    {
        id = e.Id,
        name = e.Name,
        code = e.Code,
        city = e.City ?? "",
        country = e.Country ?? "",
        isActive = e.IsActive
    };

    protected override LocationViewModel ToViewModel(Location e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Code = e.Code,
        AddressLine1 = e.AddressLine1,
        AddressLine2 = e.AddressLine2,
        City = e.City,
        State = e.State,
        Country = e.Country,
        TimeZone = e.TimeZone,
        IsHeadOffice = e.IsHeadOffice,
        IsActive = e.IsActive
    };

    protected override void Apply(LocationViewModel vm, Location e)
    {
        e.Name = vm.Name;
        e.Code = vm.Code;
        e.AddressLine1 = vm.AddressLine1;
        e.AddressLine2 = vm.AddressLine2;
        e.City = vm.City;
        e.State = vm.State;
        e.Country = vm.Country;
        e.TimeZone = vm.TimeZone;
        e.IsHeadOffice = vm.IsHeadOffice;
        e.IsActive = vm.IsActive;
    }

    protected override async Task<string?> ValidateAsync(LocationViewModel vm, int id)
        => await Repo.QueryNoTracking().AnyAsync(x => x.Code == vm.Code && x.Id != id)
            ? "A location with this code already exists."
            : null;
}
