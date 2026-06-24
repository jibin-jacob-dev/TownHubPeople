using TownHubPeople.Domain.Common;

namespace TownHubPeople.Domain.Entities.Admin;

public class RoleMenuAccess : BaseEntity
{
    public string RoleName { get; set; } = string.Empty;
    public string MenuKey { get; set; } = string.Empty;
}
