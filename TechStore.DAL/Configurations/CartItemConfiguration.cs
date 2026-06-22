using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechStore.Domain.Entities;

namespace TechStore.DAL.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems");

        builder.Property(item => item.UnitPrice).HasColumnType("decimal(18,2)");

        builder.HasIndex(item => new { item.CartId, item.ProductId, item.ProductVariantId }).IsUnique();

        builder.HasOne(item => item.Cart)
            .WithMany(cart => cart.Items)
            .HasForeignKey(item => item.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(item => item.Product)
            .WithMany(product => product.CartItems)
            .HasForeignKey(item => item.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(item => item.ProductVariant)
            .WithMany(variant => variant.CartItems)
            .HasForeignKey(item => item.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
