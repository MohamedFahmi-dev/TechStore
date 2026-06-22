using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechStore.Domain.Entities;

namespace TechStore.DAL.Configurations;

public class ProductVariantValueConfiguration : IEntityTypeConfiguration<ProductVariantValue>
{
    public void Configure(EntityTypeBuilder<ProductVariantValue> builder)
    {
        builder.ToTable("ProductVariantValues");

        builder.HasIndex(value => new { value.ProductVariantId, value.ProductVariantOptionValueId }).IsUnique();

        builder.HasOne(value => value.ProductVariant)
            .WithMany(variant => variant.ProductVariantValues)
            .HasForeignKey(value => value.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(value => value.ProductVariantOptionValue)
            .WithMany(optionValue => optionValue.ProductVariantValues)
            .HasForeignKey(value => value.ProductVariantOptionValueId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
