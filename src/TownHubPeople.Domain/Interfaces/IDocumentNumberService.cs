namespace TownHubPeople.Domain.Interfaces;

public interface IDocumentNumberService
{
    Task<string> GenerateNextNumberAsync(string prefix, CancellationToken ct = default);
}
