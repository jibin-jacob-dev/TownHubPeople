using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TownHubPeople.Domain.Entities.Admin;

namespace TownHubPeople.Infrastructure.Data.Configurations.Admin;

public class LoginPageSlideConfiguration : IEntityTypeConfiguration<LoginPageSlide>
{
    public void Configure(EntityTypeBuilder<LoginPageSlide> builder)
    {
        builder.ToTable("LoginPageSlides");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(500).IsRequired();
        builder.Property(e => e.ImageFileName).HasMaxLength(300);
        builder.Property(e => e.IconClass).HasMaxLength(100);
        builder.Property(e => e.IconColor).HasMaxLength(20);

        builder.HasIndex(e => e.SortOrder);
    }
}
