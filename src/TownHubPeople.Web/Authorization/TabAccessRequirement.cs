using Microsoft.AspNetCore.Authorization;

namespace TownHubPeople.Web.Authorization;

public class TabAccessRequirement(string moduleKey, string tabKey) : IAuthorizationRequirement
{
    public string ModuleKey { get; } = moduleKey;
    public string TabKey { get; } = tabKey;
}
