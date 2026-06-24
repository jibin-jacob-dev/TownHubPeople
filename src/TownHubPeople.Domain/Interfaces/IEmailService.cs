namespace TownHubPeople.Domain.Interfaces;

public interface IEmailService
{
    /// <summary>Queues an email into the outbox (sent later by the background sender). Does not save.</summary>
    Task QueueAsync(string toEmail, string? toName, string subject, string body, CancellationToken ct = default);
}
