using System.ComponentModel.DataAnnotations;

namespace TownHubPeople.Web.ViewModels.Admin;

public class CreateUserViewModel
{
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Display(Name = "Roles")]
    public List<string> SelectedRoles { get; set; } = [];

    public bool IsActive { get; set; } = true;

    public List<string> AvailableRoles { get; set; } = [];
}
