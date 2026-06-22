using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechStore.Domain.Entities;

namespace TechStore.DAL.Configurations;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable("Coupons");

        builder.Property(coupon => coupon.Code).HasMaxLength(50).IsRequired();
        builder.Property(coupon => coupon.Description).HasMaxLength(500);
        builder.Property(coupon => coupon.DiscountValue).HasColumnType("decimal(18,2)");
        builder.Property(coupon => coupon.MinimumOrderAmount).HasColumnType("decimal(18,2)");

        builder.HasIndex(coupon => coupon.Code).IsUnique();
    }
}
