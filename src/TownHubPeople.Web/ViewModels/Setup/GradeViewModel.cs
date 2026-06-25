using System.ComponentModel.DataAnnotations;

namespace TownHubPeople.Web.ViewModels.Setup;

public class GradeViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(30)]
    public string Code { get; set; } = string.Empty;

    [Display(Name = "Level")]
    [Range(0, int.MaxValue)]
    public int Level { get; set; }

    [Display(Name = "Min Salary")]
    [Range(0, double.MaxValue)]
    public decimal? MinSalary { get; set; }

    [Display(Name = "Max Salary")]
    [Range(0, double.MaxValue)]
    public decimal? MaxSalary { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}
