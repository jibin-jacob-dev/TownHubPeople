using TownHubPeople.Domain.Common;

namespace TownHubPeople.Domain.Entities.Admin;

public class RoleTabAccess : BaseEntity
{
    public string RoleName { get; set; } = string.Empty;
    public string ModuleKey { get; set; } = string.Empty;
    public string TabKey { get; set; } = string.Empty;
}
