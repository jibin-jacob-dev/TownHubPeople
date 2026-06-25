using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TownHubPeople.Domain.Entities.Workforce;
using TownHubPeople.Domain.Interfaces;
using TownHubPeople.Web.Authorization;
using TownHubPeople.Web.ViewModels.Setup;

namespace TownHubPeople.Web.Controllers.Setup;

[AuthorizeTab("Setup", "HolidayCalendars")]
[Route("Setup/[controller]")]
public class HolidayCalendarsController : MasterDataController<HolidayCalendar, HolidayCalendarViewModel>
{
    public HolidayCalendarsController(IUnitOfWork uow) : base(uow) { }
    protected override string ViewFolder => "~/Views/Setup/HolidayCalendars";

    protected override IQueryable<HolidayCalendar> Includes(IQueryable<HolidayCalendar> q) => q.Include(x => x.Location);

    protected override IQueryable<HolidayCalendar> Search(IQueryable<HolidayCalendar> q, string t)
        => q.Where(x => x.Name.Contains(t));

    protected override IQueryable<HolidayCalendar> Order(IQueryable<HolidayCalendar> q, string? col, bool asc) => col switch
    {
        "year" => asc ? q.OrderBy(x => x.Year) : q.OrderByDescending(x => x.Year),
        "isActive" => asc ? q.OrderBy(x => x.IsActive) : q.OrderByDescending(x => x.IsActive),
        _ => asc ? q.OrderBy(x => x.Name) : q.OrderByDescending(x => x.Name),
    };

    protected override object Project(HolidayCalendar e) => new
    {
        id = e.Id,
        name = e.Name,
        year = e.Year,
        location = e.Location != null ? e.Location.Name : "",
        isActive = e.IsActive
    };

    protected override HolidayCalendarViewModel ToViewModel(HolidayCalendar e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Year = e.Year,
        LocationId = e.LocationId,
        IsActive = e.IsActive
    };

    protected override void Apply(HolidayCalendarViewModel vm, HolidayCalendar e)
    {
        e.Name = vm.Name;
        e.Year = vm.Year;
        e.LocationId = vm.LocationId;
        e.IsActive = vm.IsActive;
    }

    protected override async Task PopulateLookupsAsync(HolidayCalendarViewModel vm)
    {
        vm.AvailableLocations = await Uow.Repository<Location>().QueryNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
            .ToListAsync();
    }

    /// <summary>Detail page: manage the holidays belonging to one calendar.</summary>
    [Route("Manage/{id:int}")]
    public async Task<IActionResult> Manage(int id)
    {
        var calendar = await Includes(Repo.QueryNoTracking()).FirstOrDefaultAsync(x => x.Id == id);
        if (calendar is null)
        {
            ErrorNotification("Holiday calendar not found.");
            return RedirectToAction(nameof(Index));
        }
        return View($"{ViewFolder}/Manage.cshtml", calendar);
    }
}
