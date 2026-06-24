using TownHubPeople.Application.Admin.AuditTrail;
using TownHubPeople.Application.Common.Models;
using TownHubPeople.Web.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TownHubPeople.Web.Controllers.Admin;

[AuthorizeTab("Administration", "AuditTrail")]
[Route("Admin/[controller]")]
public class AuditTrailController : BaseController
{
    private readonly IAuditTrailService _auditTrailService;

    private const string ViewPath = "~/Views/Admin/AuditTrail";

    public AuditTrailController(IAuditTrailService auditTrailService)
    {
        _auditTrailService = auditTrailService;
    }

    [Route("")]
    public async Task<IActionResult> Index()
    {
        ViewBag.EntityNames = await _auditTrailService.GetDistinctEntityNamesAsync();
        return View($"{ViewPath}/Index.cshtml");
    }

    [HttpPost]
    [Route("GetData")]
    public async Task<IActionResult> GetData(
        [FromBody] DataTablesRequest request,
        [FromQuery] string? entity,
        [FromQuery] string? action,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        var result = await _auditTrailService.GetDataTableAsync(request, entity, action, fromDate, toDate);
        return Json(result);
    }

    [Route("Details/{id:long}")]
    public async Task<IActionResult> Details(long id)
    {
        var entry = await _auditTrailService.GetByIdAsync(id);
        if (entry is null)
        {
            ErrorNotification("Audit trail entry not found.");
            return RedirectToAction(nameof(Index));
        }

        return View($"{ViewPath}/Details.cshtml", entry);
    }
}
