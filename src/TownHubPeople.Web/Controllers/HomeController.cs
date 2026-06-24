using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TownHubPeople.Web.Controllers;

[Authorize]
public class HomeController : BaseController
{
    public IActionResult Index()
    {
        return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(string? traceId)
    {
        ViewBag.TraceId = traceId ?? HttpContext.TraceIdentifier;
        return View();
    }
}
