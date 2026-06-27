using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TownHubPeople.Domain.Common;
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

        await SeedDocumentSequencesAsync(context);
    }

    /// <summary>Ensures the numbering sequences used by the HR domain exist (idempotent).</summary>
    private static async Task SeedDocumentSequencesAsync(ApplicationDbContext context)
    {
        if (!await context.DocumentSequences.AnyAsync(s => s.Prefix == "EMP"))
        {
            context.DocumentSequences.Add(new DocumentSequence { Prefix = "EMP", LastNumber = 0, Format = "{0}-{1:D6}" });
            await context.SaveChangesAsync();
        }
    }
}
