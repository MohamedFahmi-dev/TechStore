using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechStore.Domain.Entities;

namespace TechStore.DAL.Configurations;

public class HomepageSectionConfiguration : IEntityTypeConfiguration<HomepageSection>
{
    public void Configure(EntityTypeBuilder<HomepageSection> builder)
    {
        builder.ToTable("HomepageSections");

        builder.Property(section => section.Title).HasMaxLength(150).IsRequired();
        builder.Property(section => section.Slug).HasMaxLength(150).IsRequired();
        builder.Property(section => section.SectionType).HasMaxLength(50).IsRequired();

        builder.HasIndex(section => section.Slug).IsUnique();
    }
}
