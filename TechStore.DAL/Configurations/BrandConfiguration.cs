using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechStore.Domain.Entities;

namespace TechStore.DAL.Configurations;

public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("Brands");

        builder.Property(brand => brand.Name).HasMaxLength(100).IsRequired();
        builder.Property(brand => brand.Slug).HasMaxLength(150).IsRequired();
        builder.Property(brand => brand.Description).HasMaxLength(1000);
        builder.Property(brand => brand.LogoUrl).HasMaxLength(500);

        builder.HasIndex(brand => brand.Slug).IsUnique();
    }
}
