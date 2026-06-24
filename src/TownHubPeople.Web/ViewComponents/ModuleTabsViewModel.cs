using TownHubPeople.Application.Admin.MenuConfiguration;

namespace TownHubPeople.Web.ViewComponents;

public record TabGroupViewModel(
    string Name,
    string Icon,
    string Color,
    string FirstTabUrl,
    bool IsActive,
    IReadOnlyList<TabItemDefinition> Tabs);

public record ModuleTabsViewModel(
    bool IsGrouped,
    IReadOnlyList<TabItemDefinition> FlatTabs,
    IReadOnlyList<TabGroupViewModel> Groups);
