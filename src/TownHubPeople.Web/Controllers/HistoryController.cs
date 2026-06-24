using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TownHubPeople.Application.Admin.AuditTrail;

namespace TownHubPeople.Web.Controllers;

[Authorize]
[Route("History")]
public class HistoryController(IAuditTrailService auditTrailService) : BaseController
{
    [HttpGet("EntityHistory")]
    public async Task<IActionResult> EntityHistory(
        [FromQuery] string entityName,
        [FromQuery] string entityId,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(entityName) || string.IsNullOrWhiteSpace(entityId))
            return Json(Array.Empty<object>());

        var history = await auditTrailService.GetEntityHistoryAsync(entityName, entityId, ct);
        return Json(history);
    }
}
