namespace TownHubPeople.Domain.Entities.Admin;

using TownHubPeople.Domain.Common;
using TownHubPeople.Domain.Enums;

public class EmailOutboxMessage : BaseEntity
{
    public string ToEmail { get; set; } = string.Empty;
    public string? ToName { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public EmailStatus Status { get; set; } = EmailStatus.Pending;
    public int Attempts { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? SentDate { get; set; }
    public string? Error { get; set; }
}
