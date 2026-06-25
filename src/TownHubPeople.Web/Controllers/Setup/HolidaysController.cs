using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TownHubPeople.Application.Common.Models;
using TownHubPeople.Domain.Entities.Workforce;
using TownHubPeople.Domain.Interfaces;
using TownHubPeople.Web.Authorization;
using TownHubPeople.Web.Controllers;
using TownHubPeople.Web.ViewModels.Setup;

namespace TownHubPeople.Web.Controllers.Setup;

// Detail rows for a HolidayCalendar. Scoped by calendarId; gated by the same tab as the parent.
[AuthorizeTab("Setup", "HolidayCalendars")]
[Route("Setup/[controller]")]
public class HolidaysController : BaseController
{
    private readonly IUnitOfWork _uow;
    private const string ViewPath = "~/Views/Setup/Holidays";

    public HolidaysController(IUnitOfWork uow) => _uow = uow;

    private IRepository<Holiday> Repo => _uow.Repository<Holiday>();

    [HttpPost]
    [Route("GetData")]
    public async Task<IActionResult> GetData(int calendarId, [FromBody] DataTablesRequest request)
    {
        var baseQuery = Repo.QueryNoTracking().Where(h => h.HolidayCalendarId == calendarId);
        var total = await baseQuery.CountAsync();

        var query = baseQuery;
        var term = request.Search?.Value?.Trim();
        if (!string.IsNullOrEmpty(term)) query = query.Where(h => h.Name.Contains(term));
        var filtered = await query.CountAsync();

        var asc = request.Order.Count == 0 || request.Order[0].Dir == "asc";
        var col = request.Order.Count > 0 ? request.Columns[request.Order[0].Column].Data : null;
        query = col switch
        {
            "name" => asc ? query.OrderBy(h => h.Name) : query.OrderByDescending(h => h.Name),
            _ => asc ? query.OrderBy(h => h.Date) : query.OrderByDescending(h => h.Date),
        };

        var rows = await query.Skip(request.Start).Take(request.Length).ToListAsync();
        return Json(new DataTablesResponse<object>
        {
            Draw = request.Draw,
            RecordsTotal = total,
            RecordsFiltered = filtered,
            Data = rows.Select(h => (object)new
            {
                id = h.Id,
                date = h.Date.ToString("yyyy-MM-dd"),
                name = h.Name,
                isOptional = h.IsOptional
            }).ToList()
        });
    }

    [Route("Create")]
    public IActionResult Create(int calendarId)
        => PartialView($"{ViewPath}/_Form.cshtml", new HolidayViewModel { HolidayCalendarId = calendarId });

    [HttpPost]
    [Route("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HolidayViewModel vm)
    {
        if (!ModelState.IsValid) return Invalid(vm);

        Repo.Add(new Holiday
        {
            HolidayCalendarId = vm.HolidayCalendarId,
            Date = vm.Date,
            Name = vm.Name,
            IsOptional = vm.IsOptional
        });
        await _uow.SaveChangesAsync();
        return Saved("Holiday saved.");
    }

    [Route("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var h = await Repo.QueryNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (h is null) return RecordMissing("Holiday not found.");

        return PartialView($"{ViewPath}/_Form.cshtml", new HolidayViewModel
        {
            Id = h.Id,
            HolidayCalendarId = h.HolidayCalendarId,
            Date = h.Date,
            Name = h.Name,
            IsOptional = h.IsOptional
        });
    }

    [HttpPost]
    [Route("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, HolidayViewModel vm)
    {
        if (!ModelState.IsValid) return Invalid(vm);

        var h = await Repo.Query().FirstOrDefaultAsync(x => x.Id == id);
        if (h is null) return RecordMissing("Holiday not found.");

        h.Date = vm.Date;
        h.Name = vm.Name;
        h.IsOptional = vm.IsOptional;
        Repo.Update(h);
        await _uow.SaveChangesAsync();
        return Saved("Holiday saved.");
    }

    [HttpPost]
    [Route("Delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var h = await Repo.Query().FirstOrDefaultAsync(x => x.Id == id);
        if (h is null) return Json(new { success = false, message = "Holiday not found." });

        Repo.Remove(h);
        await _uow.SaveChangesAsync();
        return Json(new { success = true, message = "Record deleted successfully." });
    }

    private IActionResult Invalid(HolidayViewModel vm)
    {
        Response.StatusCode = StatusCodes.Status400BadRequest;
        return PartialView($"{ViewPath}/_Form.cshtml", vm);
    }

    private IActionResult Saved(string message)
    {
        if (IsAjax()) return Json(new { success = true, message });
        SuccessNotification(message);
        return RedirectToAction("Index", "HolidayCalendars");
    }

    private IActionResult RecordMissing(string message)
    {
        if (IsAjax()) return Json(new { success = false, message });
        ErrorNotification(message);
        return RedirectToAction("Index", "HolidayCalendars");
    }
}
