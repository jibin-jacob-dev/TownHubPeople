using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TownHubPeople.Web.ViewModels.Setup;

public class HolidayCalendarViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, Range(2000, 2100)]
    public int Year { get; set; } = DateTime.UtcNow.Year;

    [Display(Name = "Location")]
    public int? LocationId { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    public List<SelectListItem> AvailableLocations { get; set; } = [];
}
