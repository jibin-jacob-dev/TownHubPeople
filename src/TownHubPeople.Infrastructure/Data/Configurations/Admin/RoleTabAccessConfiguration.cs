using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TownHubPeople.Domain.Entities.Admin;

namespace TownHubPeople.Infrastructure.Data.Configurations.Admin;

public class RoleTabAccessConfiguration : IEntityTypeConfiguration<RoleTabAccess>
{
    public void Configure(EntityTypeBuilder<RoleTabAccess> builder)
    {
        builder.ToTable("RoleTabAccess");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.RoleName).HasMaxLength(256).IsRequired();
        builder.Property(e => e.ModuleKey).HasMaxLength(100).IsRequired();
        builder.Property(e => e.TabKey).HasMaxLength(100).IsRequired();

        builder.HasIndex(e => new { e.RoleName, e.ModuleKey, e.TabKey }).IsUnique();
        builder.HasIndex(e => new { e.ModuleKey, e.TabKey });
    }
}
