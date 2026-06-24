using TownHubPeople.Domain.Common;

namespace TownHubPeople.Domain.Entities.Admin;

public class TabDisplayConfig : BaseEntity
{
    public string ModuleKey { get; set; } = string.Empty;
    public string TabKey { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Icon { get; set; }
    public string? IconColor { get; set; }
    public int? SortOrder { get; set; }
}
