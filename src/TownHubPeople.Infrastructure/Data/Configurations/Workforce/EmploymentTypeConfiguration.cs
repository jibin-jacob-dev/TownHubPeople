using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TownHubPeople.Domain.Entities.Workforce;

namespace TownHubPeople.Infrastructure.Data.Configurations.Workforce;

public class EmploymentTypeConfiguration : IEntityTypeConfiguration<EmploymentType>
{
    public void Configure(EntityTypeBuilder<EmploymentType> builder)
    {
        builder.ToTable("EmploymentTypes");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Code).HasMaxLength(30).IsRequired();

        builder.HasIndex(e => e.Code).IsUnique().HasFilter("[IsDeleted] = 0");
    }
}
