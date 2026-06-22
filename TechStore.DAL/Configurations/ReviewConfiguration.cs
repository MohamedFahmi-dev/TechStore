using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechStore.Domain.Entities;

namespace TechStore.DAL.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");

        builder.Property(review => review.Comment).HasMaxLength(2000);

        builder.HasIndex(review => new { review.UserId, review.ProductId }).IsUnique();

        builder.HasOne(review => review.User)
            .WithMany(user => user.Reviews)
            .HasForeignKey(review => review.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(review => review.Product)
            .WithMany(product => product.Reviews)
            .HasForeignKey(review => review.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(review => review.Order)
            .WithMany(order => order.Reviews)
            .HasForeignKey(review => review.OrderId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
