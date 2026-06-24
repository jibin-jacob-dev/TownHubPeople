using Microsoft.AspNetCore.Mvc;

namespace TownHubPeople.Web.Authorization;

public class AuthorizeTabAttribute : TypeFilterAttribute
{
    public AuthorizeTabAttribute(string moduleKey, string tabKey)
        : base(typeof(TabAccessFilter))
    {
        Arguments = [moduleKey, tabKey];
    }
}
