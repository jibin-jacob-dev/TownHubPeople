using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TownHubPeople.Infrastructure.Data;

namespace TownHubPeople.Infrastructure.Data.Seeding;

public class ApplicationDbContextSeed(IServiceProvider serviceProvider)
{
    public async Task SeedAsync()
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        // Apply any pending migrations before seeding so the schema is always current.
        await context.Database.MigrateAsync();

        await IdentitySeed.SeedAsync(serviceProvider);

        await MenuConfigSeed.SeedAsync(context);
        await MenuConfigSeed.SeedTabsAsync(context);
        await MenuConfigSeed.EnsureAdminFullAccessAsync(context); // idempotent — grants any new modules/tabs to Administrator
    }
}
