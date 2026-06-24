using System.ComponentModel.DataAnnotations;

namespace TownHubPeople.Web.ViewModels.Admin;

public class EditUserViewModel
{
    public string Id { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [DataType(DataType.Password), MinLength(8)]
    [Display(Name = "New Password")]
    public string? NewPassword { get; set; }

    [DataType(DataType.Password), Compare(nameof(NewPassword))]
    [Display(Name = "Confirm Password")]
    public string? ConfirmPassword { get; set; }

    [Display(Name = "Roles")]
    public List<string> SelectedRoles { get; set; } = [];

    public bool IsActive { get; set; }

    public List<string> AvailableRoles { get; set; } = [];
}
