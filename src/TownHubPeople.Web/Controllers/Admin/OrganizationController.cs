using TownHubPeople.Application.Admin.Organization;
using TownHubPeople.Web.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TownHubPeople.Web.Controllers.Admin;

[AuthorizeTab("Administration", "Organization")]
[Route("Admin/[controller]")]
public class OrganizationController : BaseController
{
    private readonly IOrganizationService _service;
    private const string ViewPath = "~/Views/Admin/Organization";

    public OrganizationController(IOrganizationService service) => _service = service;

    [Route("")]
    public async Task<IActionResult> Index()
    {
        var dto = await _service.GetAsync();
        return View($"{ViewPath}/Edit.cshtml", dto);
    }

    [HttpPost]
    [Route("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(OrganizationDto dto)
    {
        if (!ModelState.IsValid)
            return View($"{ViewPath}/Edit.cshtml", dto);

        var result = await _service.UpdateAsync(dto);
        if (!result.Succeeded)
            ErrorNotification(string.Join(", ", result.Errors));
        else
            SuccessNotification("Organization profile saved.");
        return RedirectToAction(nameof(Index));
    }
}
