using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechStore.Domain.Entities;

namespace TechStore.DAL.Configurations;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");

        builder.Property(variant => variant.Name).HasMaxLength(200).IsRequired();
        builder.Property(variant => variant.SKU).HasMaxLength(100).IsRequired();
        builder.Property(variant => variant.Price).HasColumnType("decimal(18,2)");
        builder.Property(variant => variant.DiscountPrice).HasColumnType("decimal(18,2)");

        builder.HasIndex(variant => variant.SKU).IsUnique();

        builder.HasOne(variant => variant.Product)
            .WithMany(product => product.Variants)
            .HasForeignKey(variant => variant.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
