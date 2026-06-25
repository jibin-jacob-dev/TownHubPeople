using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TownHubPeople.Domain.Entities.Workforce;
using TownHubPeople.Domain.Interfaces;
using TownHubPeople.Web.Authorization;
using TownHubPeople.Web.ViewModels.Setup;

namespace TownHubPeople.Web.Controllers.Setup;

[AuthorizeTab("Setup", "Shifts")]
[Route("Setup/[controller]")]
public class ShiftsController : MasterDataController<Shift, ShiftViewModel>
{
    public ShiftsController(IUnitOfWork uow) : base(uow) { }
    protected override string ViewFolder => "~/Views/Setup/Shifts";

    protected override IQueryable<Shift> Search(IQueryable<Shift> q, string t)
        => q.Where(x => x.Name.Contains(t) || x.Code.Contains(t));

    protected override IQueryable<Shift> Order(IQueryable<Shift> q, string? col, bool asc) => col switch
    {
        "code" => asc ? q.OrderBy(x => x.Code) : q.OrderByDescending(x => x.Code),
        "startTime" => asc ? q.OrderBy(x => x.StartTime) : q.OrderByDescending(x => x.StartTime),
        "isActive" => asc ? q.OrderBy(x => x.IsActive) : q.OrderByDescending(x => x.IsActive),
        _ => asc ? q.OrderBy(x => x.Name) : q.OrderByDescending(x => x.Name),
    };

    protected override object Project(Shift e) => new
    {
        id = e.Id,
        name = e.Name,
        code = e.Code,
        startTime = e.StartTime.ToString("HH:mm"),
        endTime = e.EndTime.ToString("HH:mm"),
        isNightShift = e.IsNightShift,
        isActive = e.IsActive
    };

    protected override ShiftViewModel ToViewModel(Shift e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Code = e.Code,
        StartTime = e.StartTime,
        EndTime = e.EndTime,
        BreakMinutes = e.BreakMinutes,
        IsNightShift = e.IsNightShift,
        IsActive = e.IsActive
    };

    protected override void Apply(ShiftViewModel vm, Shift e)
    {
        e.Name = vm.Name;
        e.Code = vm.Code;
        e.StartTime = vm.StartTime;
        e.EndTime = vm.EndTime;
        e.BreakMinutes = vm.BreakMinutes;
        e.IsNightShift = vm.IsNightShift;
        e.IsActive = vm.IsActive;
    }

    protected override async Task<string?> ValidateAsync(ShiftViewModel vm, int id)
        => await Repo.QueryNoTracking().AnyAsync(x => x.Code == vm.Code && x.Id != id)
            ? "A shift with this code already exists."
            : null;
}
