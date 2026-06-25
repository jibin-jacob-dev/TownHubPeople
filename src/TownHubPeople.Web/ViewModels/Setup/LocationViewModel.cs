using System.ComponentModel.DataAnnotations;

namespace TownHubPeople.Web.ViewModels.Setup;

public class LocationViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(30)]
    public string Code { get; set; } = string.Empty;

    [Display(Name = "Address Line 1"), MaxLength(200)]
    public string? AddressLine1 { get; set; }

    [Display(Name = "Address Line 2"), MaxLength(200)]
    public string? AddressLine2 { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? State { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [Display(Name = "Time Zone"), MaxLength(100)]
    public string? TimeZone { get; set; }

    [Display(Name = "Head Office")]
    public bool IsHeadOffice { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}
