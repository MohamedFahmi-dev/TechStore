using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechStore.Domain.Entities;

namespace TechStore.DAL.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.Property(product => product.Name).HasMaxLength(200).IsRequired();
        builder.Property(product => product.Slug).HasMaxLength(200).IsRequired();
        builder.Property(product => product.SKU).HasMaxLength(100).IsRequired();
        builder.Property(product => product.ShortDescription).HasMaxLength(1000);
        builder.Property(product => product.Description).HasColumnType("nvarchar(max)");
        builder.Property(product => product.BasePrice).HasColumnType("decimal(18,2)");
        builder.Property(product => product.DiscountPrice).HasColumnType("decimal(18,2)");
        builder.Property(product => product.Condition).HasMaxLength(100);
        builder.Property(product => product.WarrantyInfo).HasMaxLength(300);

        builder.HasIndex(product => product.Slug).IsUnique();
        builder.HasIndex(product => product.SKU).IsUnique();

        builder.HasOne(product => product.Brand)
            .WithMany(brand => brand.Products)
            .HasForeignKey(product => product.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(product => product.Category)
            .WithMany(category => category.Products)
            .HasForeignKey(product => product.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
