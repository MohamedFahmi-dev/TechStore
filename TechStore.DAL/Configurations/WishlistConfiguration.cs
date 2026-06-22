using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechStore.Domain.Entities;

namespace TechStore.DAL.Configurations;

public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        builder.ToTable("Wishlists");

        builder.HasIndex(wishlist => new { wishlist.UserId, wishlist.ProductId }).IsUnique();

        builder.HasOne(wishlist => wishlist.User)
            .WithMany(user => user.WishlistItems)
            .HasForeignKey(wishlist => wishlist.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(wishlist => wishlist.Product)
            .WithMany(product => product.WishlistItems)
            .HasForeignKey(wishlist => wishlist.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
