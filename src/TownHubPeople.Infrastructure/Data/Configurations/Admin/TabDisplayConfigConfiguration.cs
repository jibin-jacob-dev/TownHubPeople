using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TownHubPeople.Domain.Entities.Admin;

namespace TownHubPeople.Infrastructure.Data.Configurations.Admin;

public class TabDisplayConfigConfiguration : IEntityTypeConfiguration<TabDisplayConfig>
{
    public void Configure(EntityTypeBuilder<TabDisplayConfig> builder)
    {
        builder.ToTable("TabDisplayConfigs");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.ModuleKey).HasMaxLength(100).IsRequired();
        builder.Property(e => e.TabKey).HasMaxLength(100).IsRequired();
        builder.Property(e => e.DisplayName).HasMaxLength(200);
        builder.Property(e => e.Icon).HasMaxLength(100);
        builder.Property(e => e.IconColor).HasMaxLength(20);

        builder.HasIndex(e => new { e.ModuleKey, e.TabKey }).IsUnique();
    }
}
