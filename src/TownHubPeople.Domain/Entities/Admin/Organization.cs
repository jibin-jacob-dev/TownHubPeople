namespace TownHubPeople.Domain.Entities.Admin;

using TownHubPeople.Domain.Common;

/// <summary>Singleton organization profile shown on payslips and reports.</summary>
public class Organization : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    /// <summary>PAN / GSTIN or equivalent tax registration number.</summary>
    public string? TaxId { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public string CurrencySymbol { get; set; } = "₹";
    public string? LogoUrl { get; set; }
}
