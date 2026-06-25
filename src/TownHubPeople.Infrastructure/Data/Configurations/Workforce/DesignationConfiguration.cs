using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TownHubPeople.Domain.Entities.Workforce;

namespace TownHubPeople.Infrastructure.Data.Configurations.Workforce;

public class DesignationConfiguration : IEntityTypeConfiguration<Designation>
{
    public void Configure(EntityTypeBuilder<Designation> builder)
    {
        builder.ToTable("Designations");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).HasMaxLength(150).IsRequired();
        builder.Property(e => e.Code).HasMaxLength(30).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(500);

        builder.HasIndex(e => e.Code).IsUnique().HasFilter("[IsDeleted] = 0");

        builder.HasOne(e => e.Grade)
            .WithMany()
            .HasForeignKey(e => e.GradeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
