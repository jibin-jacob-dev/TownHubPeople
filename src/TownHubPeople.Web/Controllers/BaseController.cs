using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace TownHubPeople.Web.Controllers;

public abstract class BaseController : Controller
{
    protected void SuccessNotification(string message)
        => TempData["Notification"] = JsonSerializer.Serialize(new { type = "success", message });

    protected void ErrorNotification(string message)
        => TempData["Notification"] = JsonSerializer.Serialize(new { type = "error", message });

    protected void WarningNotification(string message)
        => TempData["Notification"] = JsonSerializer.Serialize(new { type = "warning", message });

    protected void InfoNotification(string message)
        => TempData["Notification"] = JsonSerializer.Serialize(new { type = "info", message });
}
