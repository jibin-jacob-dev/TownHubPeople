using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TownHubPeople.Domain.Entities.Workforce;

namespace TownHubPeople.Infrastructure.Data.Configurations.Workforce;

public class GradeConfiguration : IEntityTypeConfiguration<Grade>
{
    public void Configure(EntityTypeBuilder<Grade> builder)
    {
        builder.ToTable("Grades");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).HasMaxLength(150).IsRequired();
        builder.Property(e => e.Code).HasMaxLength(30).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.MinSalary).HasPrecision(18, 2);
        builder.Property(e => e.MaxSalary).HasPrecision(18, 2);

        builder.HasIndex(e => e.Code).IsUnique().HasFilter("[IsDeleted] = 0");
    }
}
