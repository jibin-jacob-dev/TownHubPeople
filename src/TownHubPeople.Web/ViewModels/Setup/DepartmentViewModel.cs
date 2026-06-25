using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TownHubPeople.Web.ViewModels.Setup;

public class DepartmentViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(30)]
    public string Code { get; set; } = string.Empty;

    [Display(Name = "Parent Department")]
    public int? ParentDepartmentId { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    public List<SelectListItem> AvailableParents { get; set; } = [];
}
