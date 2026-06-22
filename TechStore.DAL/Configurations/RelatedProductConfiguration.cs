using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechStore.Domain.Entities;

namespace TechStore.DAL.Configurations;

public class RelatedProductConfiguration : IEntityTypeConfiguration<RelatedProduct>
{
    public void Configure(EntityTypeBuilder<RelatedProduct> builder)
    {
        builder.ToTable("RelatedProducts");

        builder.HasIndex(related => new { related.ProductId, related.RelatedProductId, related.RelationType }).IsUnique();

        builder.HasOne(related => related.Product)
            .WithMany(product => product.RelatedProducts)
            .HasForeignKey(related => related.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(related => related.RelatedItem)
            .WithMany(product => product.RelatedToProducts)
            .HasForeignKey(related => related.RelatedProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
