using System.ComponentModel.DataAnnotations;

namespace TownHubPeople.Web.ViewModels.Setup;

public class HolidayViewModel
{
    public int Id { get; set; }

    [Required]
    public int HolidayCalendarId { get; set; }

    [Required, DataType(DataType.Date)]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Optional Holiday")]
    public bool IsOptional { get; set; }
}
