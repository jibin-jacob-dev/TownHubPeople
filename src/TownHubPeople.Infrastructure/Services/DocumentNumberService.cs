using Microsoft.EntityFrameworkCore;
using TownHubPeople.Domain.Common;
using TownHubPeople.Domain.Interfaces;
using TownHubPeople.Infrastructure.Data;

namespace TownHubPeople.Infrastructure.Services;

public class DocumentNumberService(ApplicationDbContext context) : IDocumentNumberService
{
    public async Task<string> GenerateNextNumberAsync(string prefix, CancellationToken ct = default)
    {
        var sequence = await context.DocumentSequences
            .FromSqlInterpolated($"SELECT * FROM DocumentSequences WITH (UPDLOCK, ROWLOCK) WHERE Prefix = {prefix}")
            .FirstOrDefaultAsync(ct)
            ?? throw new InvalidOperationException($"Document sequence for prefix '{prefix}' not found.");

        sequence.LastNumber++;
        context.DocumentSequences.Update(sequence);
        await context.SaveChangesAsync(ct);

        return string.Format(sequence.Format, prefix, sequence.LastNumber);
    }
}
