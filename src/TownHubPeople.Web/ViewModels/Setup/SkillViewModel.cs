using System.ComponentModel.DataAnnotations;

namespace TownHubPeople.Web.ViewModels.Setup;

public class SkillViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Category { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}
