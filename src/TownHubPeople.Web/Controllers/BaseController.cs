using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace TownHubPeople.Web.Controllers;

public abstract class BaseController : Controller
{
    /// <summary>
    /// True when the request was issued via fetch/XHR (popup forms set the
    /// X-Requested-With header). Controllers use this to return form partials /
    /// JSON instead of full page views + redirects.
    /// </summary>
    protected bool IsAjax()
        => Request.Headers["X-Requested-With"] == "XMLHttpRequest";

    protected void SuccessNotification(string message)
        => TempData["Notification"] = JsonSerializer.Serialize(new { type = "success", message });

    protected void ErrorNotification(string message)
        => TempData["Notification"] = JsonSerializer.Serialize(new { type = "error", message });

    protected void WarningNotification(string message)
        => TempData["Notification"] = JsonSerializer.Serialize(new { type = "warning", message });

    protected void InfoNotification(string message)
        => TempData["Notification"] = JsonSerializer.Serialize(new { type = "info", message });
}
