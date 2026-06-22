using TechStore.DAL.Generic;
using TechStore.Domain.Entities;

namespace TechStore.DAL.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepositoryAsync<Brand> Brands { get; }
        IGenericRepositoryAsync<Category> Categories { get; }
        IGenericRepositoryAsync<Product> Products { get; }
        IGenericRepositoryAsync<ProductImage> ProductImages { get; }
        IGenericRepositoryAsync<ProductSpec> ProductSpecs { get; }
        IGenericRepositoryAsync<ProductVariant> ProductVariants { get; }
        IGenericRepositoryAsync<ProductVariantOption> ProductVariantOptions { get; }
        IGenericRepositoryAsync<ProductVariantOptionValue> ProductVariantOptionValues { get; }
        IGenericRepositoryAsync<ProductVariantValue> ProductVariantValues { get; }
        IGenericRepositoryAsync<RelatedProduct> RelatedProducts { get; }

        // Cart
        IGenericRepositoryAsync<Cart> Carts { get; }
        IGenericRepositoryAsync<CartItem> CartItems { get; }

        // Orders
        IGenericRepositoryAsync<Order> Orders { get; }
        IGenericRepositoryAsync<OrderItem> OrderItems { get; }
        IGenericRepositoryAsync<Payment> Payments { get; }

        // Users
        IGenericRepositoryAsync<Address> Addresses { get; }

        // Social / CMS
        IGenericRepositoryAsync<Review> Reviews { get; }
        IGenericRepositoryAsync<Wishlist> Wishlists { get; }
        IGenericRepositoryAsync<Notification> Notifications { get; }
        IGenericRepositoryAsync<NewsletterSubscriber> NewsletterSubscribers { get; }
        IGenericRepositoryAsync<HomepageSection> HomepageSections { get; }
        IGenericRepositoryAsync<HomepageSectionItem> HomepageSectionItems { get; }
        IGenericRepositoryAsync<Coupon> Coupons { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
