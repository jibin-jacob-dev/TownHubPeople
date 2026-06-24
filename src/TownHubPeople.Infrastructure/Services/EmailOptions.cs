namespace TownHubPeople.Infrastructure.Services;

public class EmailOptions
{
    /// <summary>When false, the outbox still records messages but no SMTP send is attempted (they are marked Sent).</summary>
    public bool Enabled { get; set; }
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public bool UseSsl { get; set; } = true;
    public string FromAddress { get; set; } = "no-reply@townhubpeople.example.in";
    public string FromName { get; set; } = "TownHub People";
    public string? UserName { get; set; }
    public string? Password { get; set; }
}
