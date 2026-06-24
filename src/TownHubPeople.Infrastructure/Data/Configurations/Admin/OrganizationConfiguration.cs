using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TownHubPeople.Domain.Entities.Admin;

namespace TownHubPeople.Infrastructure.Data.Configurations.Admin;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organizations");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.Property(e => e.LegalName).HasMaxLength(200);
        builder.Property(e => e.AddressLine1).HasMaxLength(200);
        builder.Property(e => e.AddressLine2).HasMaxLength(200);
        builder.Property(e => e.City).HasMaxLength(100);
        builder.Property(e => e.State).HasMaxLength(100);
        builder.Property(e => e.PostalCode).HasMaxLength(20);
        builder.Property(e => e.Country).HasMaxLength(100);
        builder.Property(e => e.Phone).HasMaxLength(50);
        builder.Property(e => e.Email).HasMaxLength(200);
        builder.Property(e => e.TaxId).HasMaxLength(50);
        builder.Property(e => e.CurrencyCode).HasMaxLength(10);
        builder.Property(e => e.CurrencySymbol).HasMaxLength(10);
        builder.Property(e => e.LogoUrl).HasMaxLength(500);
    }
}
