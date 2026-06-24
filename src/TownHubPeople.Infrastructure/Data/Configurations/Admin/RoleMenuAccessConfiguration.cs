using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TownHubPeople.Domain.Entities.Admin;

namespace TownHubPeople.Infrastructure.Data.Configurations.Admin;

public class RoleMenuAccessConfiguration : IEntityTypeConfiguration<RoleMenuAccess>
{
    public void Configure(EntityTypeBuilder<RoleMenuAccess> builder)
    {
        builder.ToTable("RoleMenuAccess");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.RoleName).HasMaxLength(256).IsRequired();
        builder.Property(e => e.MenuKey).HasMaxLength(100).IsRequired();

        builder.HasIndex(e => new { e.RoleName, e.MenuKey }).IsUnique();
        builder.HasIndex(e => e.MenuKey);
    }
}
