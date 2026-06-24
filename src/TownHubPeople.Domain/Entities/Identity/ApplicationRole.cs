using Microsoft.AspNetCore.Identity;

namespace TownHubPeople.Domain.Entities.Identity;

public class ApplicationRole : IdentityRole
{
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
}
