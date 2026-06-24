using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TownHubPeople.Domain.Enums;
using TownHubPeople.Infrastructure.Data;

namespace TownHubPeople.Infrastructure.Services;

/// <summary>Background worker that delivers queued outbox emails (or marks them sent when SMTP is disabled).</summary>
public class EmailOutboxSender : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(30);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly EmailOptions _options;
    private readonly ILogger<EmailOutboxSender> _logger;

    public EmailOutboxSender(IServiceScopeFactory scopeFactory, IOptions<EmailOptions> options, ILogger<EmailOutboxSender> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email outbox processing failed.");
            }

            try { await Task.Delay(Interval, stoppingToken); }
            catch (TaskCanceledException) { break; }
        }
    }

    private async Task ProcessBatchAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var pending = await context.EmailOutbox
            .Where(m => m.Status == EmailStatus.Pending && m.Attempts < 5)
            .OrderBy(m => m.Id)
            .Take(20)
            .ToListAsync(ct);

        if (pending.Count == 0) return;

        foreach (var msg in pending)
        {
            msg.Attempts++;
            try
            {
                if (_options.Enabled)
                    await SendAsync(msg.ToEmail, msg.ToName, msg.Subject, msg.Body, ct);
                else
                    _logger.LogInformation("Email (SMTP disabled) to {To}: {Subject}", msg.ToEmail, msg.Subject);

                msg.Status = EmailStatus.Sent;
                msg.SentDate = DateTime.UtcNow;
                msg.Error = null;
            }
            catch (Exception ex)
            {
                msg.Status = msg.Attempts >= 5 ? EmailStatus.Failed : EmailStatus.Pending;
                msg.Error = ex.Message;
                _logger.LogWarning(ex, "Failed to send email {Id} to {To}.", msg.Id, msg.ToEmail);
            }
        }

        await context.SaveChangesAsync(ct);
    }

    private async Task SendAsync(string to, string? toName, string subject, string body, CancellationToken ct)
    {
        using var client = new SmtpClient(_options.Host, _options.Port) { EnableSsl = _options.UseSsl };
        if (!string.IsNullOrEmpty(_options.UserName))
            client.Credentials = new NetworkCredential(_options.UserName, _options.Password);

        using var message = new MailMessage
        {
            From = new MailAddress(_options.FromAddress, _options.FromName),
            Subject = subject,
            Body = body
        };
        message.To.Add(new MailAddress(to, toName ?? to));
        await client.SendMailAsync(message, ct);
    }
}
