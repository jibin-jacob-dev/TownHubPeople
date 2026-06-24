using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TownHubPeople.Domain.Common;

namespace TownHubPeople.Infrastructure.Data.Configurations;

public class DocumentSequenceConfiguration : IEntityTypeConfiguration<DocumentSequence>
{
    public void Configure(EntityTypeBuilder<DocumentSequence> builder)
    {
        builder.ToTable("DocumentSequences");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Prefix).HasMaxLength(10).IsRequired();
        builder.Property(e => e.Format).HasMaxLength(50).IsRequired();
        builder.HasIndex(e => e.Prefix).IsUnique();
    }
}
