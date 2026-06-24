using TownHubPeople.Domain.Entities.Admin;
using TownHubPeople.Domain.Enums;
using TownHubPeople.Domain.Interfaces;
using TownHubPeople.Infrastructure.Data;

namespace TownHubPeople.Infrastructure.Services;

/// <summary>Queues outbound emails into the database outbox. Delivery is handled by EmailOutboxSender.</summary>
public class EmailService : IEmailService
{
    private readonly ApplicationDbContext _context;

    public EmailService(ApplicationDbContext context) => _context = context;

    public Task QueueAsync(string toEmail, string? toName, string subject, string body, CancellationToken ct = default)
    {
        _context.EmailOutbox.Add(new EmailOutboxMessage
        {
            ToEmail = toEmail,
            ToName = toName,
            Subject = subject,
            Body = body,
            Status = EmailStatus.Pending,
            CreatedDate = DateTime.UtcNow
        });
        // Saved by the caller's unit of work.
        return Task.CompletedTask;
    }
}
