using TownHubPeople.Domain.Common;

namespace TownHubPeople.Domain.Entities.Admin;

public class LoginPageSlide : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageFileName { get; set; }
    public string IconClass { get; set; } = "fa-chart-line";
    public string IconColor { get; set; } = "#3b82f6";
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public bool ShowOnLogin { get; set; } = true;
    public bool ShowOnRegister { get; set; } = true;
}
