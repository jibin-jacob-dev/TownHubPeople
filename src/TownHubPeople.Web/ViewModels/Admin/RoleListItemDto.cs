namespace TownHubPeople.Web.ViewModels.Admin;

public class RoleListItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int UserCount { get; set; }
    public DateTime CreatedDate { get; set; }
}
