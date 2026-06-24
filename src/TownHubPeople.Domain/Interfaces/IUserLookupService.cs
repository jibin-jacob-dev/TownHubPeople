namespace TownHubPeople.Domain.Interfaces;

public record UserContact(string? Email, string? Name);

public interface IUserLookupService
{
    Task<UserContact?> FindAsync(string userId, CancellationToken ct = default);
}
