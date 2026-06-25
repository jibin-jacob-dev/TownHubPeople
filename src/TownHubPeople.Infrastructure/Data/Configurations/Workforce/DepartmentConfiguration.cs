using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TownHubPeople.Domain.Entities.Workforce;

namespace TownHubPeople.Infrastructure.Data.Configurations.Workforce;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).HasMaxLength(150).IsRequired();
        builder.Property(e => e.Code).HasMaxLength(30).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(500);

        // Unique among live (non-deleted) rows only — soft-deleted codes can be reused.
        builder.HasIndex(e => e.Code).IsUnique().HasFilter("[IsDeleted] = 0");

        builder.HasOne(e => e.ParentDepartment)
            .WithMany()
            .HasForeignKey(e => e.ParentDepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
