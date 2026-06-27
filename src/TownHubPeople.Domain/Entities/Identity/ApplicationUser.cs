using Microsoft.AspNetCore.Identity;

namespace TownHubPeople.Domain.Entities.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public string? ProfileImageUrl { get; set; }

    public Entities.People.Employee? Employee { get; set; }
}
