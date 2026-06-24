namespace TownHubPeople.Web.ViewModels.Account;

public class ProfileViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = [];
    public DateTime CreatedDate { get; set; }
    public string? ProfileImageUrl { get; set; }

    // Reserved for HR linkage — populated once the Employee domain is added.
    public bool HasLinkedEmployee { get; set; }
    public string? EmployeeNumber { get; set; }
    public string? DepartmentName { get; set; }
    public string? Designation { get; set; }
    public DateTime? HireDate { get; set; }
}
