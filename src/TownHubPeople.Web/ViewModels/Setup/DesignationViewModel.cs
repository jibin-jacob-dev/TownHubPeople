using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TownHubPeople.Web.ViewModels.Setup;

public class DesignationViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(30)]
    public string Code { get; set; } = string.Empty;

    [Display(Name = "Grade")]
    public int? GradeId { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    public List<SelectListItem> AvailableGrades { get; set; } = [];
}
