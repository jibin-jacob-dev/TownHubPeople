using System.ComponentModel.DataAnnotations;

namespace TownHubPeople.Web.ViewModels.Admin;

public class LoginSlideViewModel
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public string IconClass { get; set; } = "fa-chart-line";

    public string IconColor { get; set; } = "#3b82f6";

    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public bool ShowOnLogin { get; set; } = true;

    public bool ShowOnRegister { get; set; } = true;

    public string? ExistingImageFileName { get; set; }
}
