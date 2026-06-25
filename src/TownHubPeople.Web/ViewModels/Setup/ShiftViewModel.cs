using System.ComponentModel.DataAnnotations;

namespace TownHubPeople.Web.ViewModels.Setup;

public class ShiftViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(30)]
    public string Code { get; set; } = string.Empty;

    [Display(Name = "Start Time"), DataType(DataType.Time)]
    public TimeOnly StartTime { get; set; }

    [Display(Name = "End Time"), DataType(DataType.Time)]
    public TimeOnly EndTime { get; set; }

    [Display(Name = "Break (minutes)"), Range(0, 1440)]
    public int BreakMinutes { get; set; }

    [Display(Name = "Night Shift")]
    public bool IsNightShift { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}
