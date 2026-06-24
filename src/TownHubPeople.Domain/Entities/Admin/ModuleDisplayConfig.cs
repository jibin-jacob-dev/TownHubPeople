using TownHubPeople.Domain.Common;

namespace TownHubPeople.Domain.Entities.Admin;

public class ModuleDisplayConfig : BaseEntity
{
    public string ModuleKey { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public int? SortOrder { get; set; }
}
