using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TownHubPeople.Domain.Entities.Admin;

namespace TownHubPeople.Infrastructure.Data.Configurations.Admin;

public class ModuleDisplayConfigConfiguration : IEntityTypeConfiguration<ModuleDisplayConfig>
{
    public void Configure(EntityTypeBuilder<ModuleDisplayConfig> builder)
    {
        builder.ToTable("ModuleDisplayConfigs");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.ModuleKey).HasMaxLength(100).IsRequired();
        builder.Property(e => e.DisplayName).HasMaxLength(200);

        builder.HasIndex(e => e.ModuleKey).IsUnique();
    }
}
