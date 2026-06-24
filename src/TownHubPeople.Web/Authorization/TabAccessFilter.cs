using System.Security.Claims;
using TownHubPeople.Application.Admin.MenuConfiguration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TownHubPeople.Web.Authorization;

public class TabAccessFilter(
    IMenuConfigurationService menuConfigService,
    string moduleKey,
    string tabKey) : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (user.Identity?.IsAuthenticated != true)
        {
            context.Result = new ChallengeResult();
            return;
        }

        var roles = user.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        var accessibleTabs = await menuConfigService.GetAccessibleTabsAsync(moduleKey, roles);

        if (!accessibleTabs.Any(t => t.TabKey == tabKey))
            context.Result = new ForbidResult();
    }
}
