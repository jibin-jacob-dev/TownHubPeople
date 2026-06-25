using System.ComponentModel.DataAnnotations;

namespace TownHubPeople.Web.ViewModels.Setup;

public class EmploymentTypeViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(30)]
    public string Code { get; set; } = string.Empty;

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}
