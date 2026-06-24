using System.Security.Claims;
using TownHubPeople.Application.Admin.MenuConfiguration;
using Microsoft.AspNetCore.Authorization;

namespace TownHubPeople.Web.Authorization;

public class TabAccessHandler(IServiceScopeFactory scopeFactory) : AuthorizationHandler<TabAccessRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TabAccessRequirement requirement)
    {
        var roles = context.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        if (roles.Count == 0)
            return;

        using var scope = scopeFactory.CreateScope();
        var menuService = scope.ServiceProvider.GetRequiredService<IMenuConfigurationService>();
        var accessibleTabs = await menuService.GetAccessibleTabsAsync(requirement.ModuleKey, roles);

        if (accessibleTabs.Any(t => t.TabKey == requirement.TabKey))
            context.Succeed(requirement);
    }
}
