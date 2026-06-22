using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechStore.Domain.Entities;

namespace TechStore.DAL.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.Property(payment => payment.Method).HasMaxLength(50).IsRequired();
        builder.Property(payment => payment.TransactionId).HasMaxLength(200);
        builder.Property(payment => payment.Amount).HasColumnType("decimal(18,2)");

        builder.HasIndex(payment => payment.OrderId).IsUnique();
    }
}
