using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechStore.Domain.Entities;

namespace TechStore.DAL.Configurations;

public class ProductVariantOptionValueConfiguration : IEntityTypeConfiguration<ProductVariantOptionValue>
{
    public void Configure(EntityTypeBuilder<ProductVariantOptionValue> builder)
    {
        builder.ToTable("ProductVariantOptionValues");

        builder.Property(value => value.Value).HasMaxLength(100).IsRequired();

        builder.HasIndex(value => new { value.ProductVariantOptionId, value.Value }).IsUnique();

        builder.HasOne(value => value.ProductVariantOption)
            .WithMany(option => option.Values)
            .HasForeignKey(value => value.ProductVariantOptionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
