using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechStore.Domain.Entities;

namespace TechStore.DAL.Configurations;

public class ProductVariantOptionConfiguration : IEntityTypeConfiguration<ProductVariantOption>
{
    public void Configure(EntityTypeBuilder<ProductVariantOption> builder)
    {
        builder.ToTable("ProductVariantOptions");

        builder.Property(option => option.Name).HasMaxLength(100).IsRequired();

        builder.HasIndex(option => new { option.ProductId, option.Name }).IsUnique();

        builder.HasOne(option => option.Product)
            .WithMany(product => product.VariantOptions)
            .HasForeignKey(option => option.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
