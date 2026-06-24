using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TownHubPeople.Domain.Common;

namespace TownHubPeople.Infrastructure.Data.Configurations;

public class AuditTrailEntryConfiguration : IEntityTypeConfiguration<AuditTrailEntry>
{
    public void Configure(EntityTypeBuilder<AuditTrailEntry> builder)
    {
        builder.ToTable("AuditTrailEntries");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityColumn();
        builder.Property(e => e.UserId).HasMaxLength(450).IsRequired();
        builder.Property(e => e.UserName).HasMaxLength(256).IsRequired();
        builder.Property(e => e.EntityName).HasMaxLength(256).IsRequired();
        builder.Property(e => e.EntityId).HasMaxLength(256).IsRequired();
        builder.Property(e => e.Action).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Timestamp).IsRequired();

        builder.HasIndex(e => e.EntityName);
        builder.HasIndex(e => e.Timestamp);
        builder.HasIndex(e => e.UserId);
    }
}
