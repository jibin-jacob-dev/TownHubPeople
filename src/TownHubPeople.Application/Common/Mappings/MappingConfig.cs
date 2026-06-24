using Mapster;

namespace TownHubPeople.Application.Common.Mappings;

public static class MappingConfig
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig.GlobalSettings.Default
            .IgnoreNullValues(true);
    }
}
