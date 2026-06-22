using TechStore.DAL.Context;
using TechStore.DAL.Generic;
using TechStore.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace TechStore.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TechDbContext _context;
        private IDbContextTransaction _currentTransaction;

        // Catalog
        public IGenericRepositoryAsync<Brand> Brands { get; private set; }
        public IGenericRepositoryAsync<Category> Categories { get; private set; }
        public IGenericRepositoryAsync<Product> Products { get; private set; }
        public IGenericRepositoryAsync<ProductImage> ProductImages { get; private set; }
        public IGenericRepositoryAsync<ProductSpec> ProductSpecs { get; private set; }
        public IGenericRepositoryAsync<ProductVariant> ProductVariants { get; private set; }
        public IGenericRepositoryAsync<ProductVariantOption> ProductVariantOptions { get; private set; }
        public IGenericRepositoryAsync<ProductVariantOptionValue> ProductVariantOptionValues { get; private set; }
        public IGenericRepositoryAsync<ProductVariantValue> ProductVariantValues { get; private set; }
        public IGenericRepositoryAsync<RelatedProduct> RelatedProducts { get; private set; }

        // Cart
        public IGenericRepositoryAsync<Cart> Carts { get; private set; }
        public IGenericRepositoryAsync<CartItem> CartItems { get; private set; }

        // Orders
        public IGenericRepositoryAsync<Order> Orders { get; private set; }
        public IGenericRepositoryAsync<OrderItem> OrderItems { get; private set; }
        public IGenericRepositoryAsync<Payment> Payments { get; private set; }

        // Users
        public IGenericRepositoryAsync<Address> Addresses { get; private set; }

        // Social 
        public IGenericRepositoryAsync<Review> Reviews { get; private set; }
        public IGenericRepositoryAsync<Wishlist> Wishlists { get; private set; }
        public IGenericRepositoryAsync<Notification> Notifications { get; private set; }
        public IGenericRepositoryAsync<NewsletterSubscriber> NewsletterSubscribers { get; private set; }
        public IGenericRepositoryAsync<HomepageSection> HomepageSections { get; private set; }
        public IGenericRepositoryAsync<HomepageSectionItem> HomepageSectionItems { get; private set; }
        public IGenericRepositoryAsync<Coupon> Coupons { get; private set; }

        public UnitOfWork(TechDbContext context)
        {
            _context = context;

            // Catalog
            Brands = new GenericRepositoryAsync<Brand>(context);
            Categories = new GenericRepositoryAsync<Category>(context);
            Products = new GenericRepositoryAsync<Product>(context);
            ProductImages = new GenericRepositoryAsync<ProductImage>(context);
            ProductSpecs = new GenericRepositoryAsync<ProductSpec>(context);
            ProductVariants = new GenericRepositoryAsync<ProductVariant>(context);
            ProductVariantOptions = new GenericRepositoryAsync<ProductVariantOption>(context);
            ProductVariantOptionValues = new GenericRepositoryAsync<ProductVariantOptionValue>(context);
            ProductVariantValues = new GenericRepositoryAsync<ProductVariantValue>(context);
            RelatedProducts = new GenericRepositoryAsync<RelatedProduct>(context);

            // Cart
            Carts = new GenericRepositoryAsync<Cart>(context);
            CartItems = new GenericRepositoryAsync<CartItem>(context);

            // Orders
            Orders = new GenericRepositoryAsync<Order>(context);
            OrderItems = new GenericRepositoryAsync<OrderItem>(context);
            Payments = new GenericRepositoryAsync<Payment>(context);

            // Users
            Addresses = new GenericRepositoryAsync<Address>(context);

            // Social 
            Reviews = new GenericRepositoryAsync<Review>(context);
            Wishlists = new GenericRepositoryAsync<Wishlist>(context);
            Notifications = new GenericRepositoryAsync<Notification>(context);
            NewsletterSubscribers = new GenericRepositoryAsync<NewsletterSubscriber>(context);
            HomepageSections = new GenericRepositoryAsync<HomepageSection>(context);
            HomepageSectionItems = new GenericRepositoryAsync<HomepageSectionItem>(context);
            Coupons = new GenericRepositoryAsync<Coupon>(context);
        }

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null) return;
            _currentTransaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_currentTransaction != null)
                    await _currentTransaction.CommitAsync();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                if (_currentTransaction != null)
                    await _currentTransaction.RollbackAsync();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }

        public void Dispose()
        {
            _currentTransaction?.Dispose();
            _context.Dispose();
        }
    }
}
