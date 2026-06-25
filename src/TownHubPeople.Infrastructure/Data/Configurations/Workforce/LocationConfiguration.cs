using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TownHubPeople.Domain.Entities.Workforce;

namespace TownHubPeople.Infrastructure.Data.Configurations.Workforce;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Locations");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).HasMaxLength(150).IsRequired();
        builder.Property(e => e.Code).HasMaxLength(30).IsRequired();
        builder.Property(e => e.AddressLine1).HasMaxLength(200);
        builder.Property(e => e.AddressLine2).HasMaxLength(200);
        builder.Property(e => e.City).HasMaxLength(100);
        builder.Property(e => e.State).HasMaxLength(100);
        builder.Property(e => e.Country).HasMaxLength(100);
        builder.Property(e => e.TimeZone).HasMaxLength(100);

        builder.HasIndex(e => e.Code).IsUnique().HasFilter("[IsDeleted] = 0");
    }
}
