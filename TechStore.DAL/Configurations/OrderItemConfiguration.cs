using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechStore.Domain.Entities;

namespace TechStore.DAL.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.Property(item => item.ProductName).HasMaxLength(200).IsRequired();
        builder.Property(item => item.SKU).HasMaxLength(100).IsRequired();
        builder.Property(item => item.VariantLabel).HasMaxLength(300);
        builder.Property(item => item.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Property(item => item.TotalPrice).HasColumnType("decimal(18,2)");

        builder.HasOne(item => item.Order)
            .WithMany(order => order.Items)
            .HasForeignKey(item => item.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(item => item.Product)
            .WithMany(product => product.OrderItems)
            .HasForeignKey(item => item.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(item => item.ProductVariant)
            .WithMany(variant => variant.OrderItems)
            .HasForeignKey(item => item.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
