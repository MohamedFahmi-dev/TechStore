using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechStore.Domain.Entities;

namespace TechStore.DAL.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.Property(order => order.OrderNumber).HasMaxLength(50).IsRequired();
        builder.Property(order => order.Subtotal).HasColumnType("decimal(18,2)");
        builder.Property(order => order.DiscountAmount).HasColumnType("decimal(18,2)");
        builder.Property(order => order.ShippingFee).HasColumnType("decimal(18,2)");
        builder.Property(order => order.TotalAmount).HasColumnType("decimal(18,2)");
        builder.Property(order => order.Notes).HasMaxLength(1000);

        builder.HasIndex(order => order.OrderNumber).IsUnique();

        builder.HasOne(order => order.User)
            .WithMany(user => user.Orders)
            .HasForeignKey(order => order.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(order => order.Address)
            .WithMany(address => address.Orders)
            .HasForeignKey(order => order.AddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(order => order.Coupon)
            .WithMany(coupon => coupon.Orders)
            .HasForeignKey(order => order.CouponId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(order => order.Payment)
            .WithOne(payment => payment.Order)
            .HasForeignKey<Payment>(payment => payment.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
