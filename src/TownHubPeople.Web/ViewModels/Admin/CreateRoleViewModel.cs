using System.ComponentModel.DataAnnotations;

namespace TownHubPeople.Web.ViewModels.Admin;

public class CreateRoleViewModel
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    // Optional: copy from an existing role OR apply a preset (CopyFromRole takes priority if both set)
    public string? CopyFromRole { get; set; }
    public string? ApplyPreset { get; set; }
}
