using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechStore.Domain.Common;
using TechStore.Domain.Entities;

namespace TechStore.DAL.Context;

public class TechDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public TechDbContext(DbContextOptions<TechDbContext> options) : base(options)
    {
    }

    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<HomepageSection> HomepageSections => Set<HomepageSection>();
    public DbSet<HomepageSectionItem> HomepageSectionItems => Set<HomepageSectionItem>();
    public DbSet<NewsletterSubscriber> NewsletterSubscribers => Set<NewsletterSubscriber>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ProductSpec> ProductSpecs => Set<ProductSpec>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<ProductVariantOption> ProductVariantOptions => Set<ProductVariantOption>();
    public DbSet<ProductVariantOptionValue> ProductVariantOptionValues => Set<ProductVariantOptionValue>();
    public DbSet<ProductVariantValue> ProductVariantValues => Set<ProductVariantValue>();
    public DbSet<RelatedProduct> RelatedProducts => Set<RelatedProduct>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Wishlist> Wishlists => Set<Wishlist>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<IdentityRole<int>>().ToTable("Roles");
        builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");

        builder.ApplyConfigurationsFromAssembly(typeof(TechDbContext).Assembly);
        ApplySoftDeleteQueryFilters(builder);
    }

    public override int SaveChanges()
    {
        ApplyAuditRules();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyAuditRules();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditRules();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyAuditRules();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyAuditRules()
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is IAuditableEntity auditableEntity)
            {
                if (entry.State == EntityState.Added)
                {
                    auditableEntity.CreatedOnUtc = utcNow;
                    auditableEntity.ModifiedOnUtc = null;
                }
                else if (entry.State == EntityState.Modified)
                {
                    auditableEntity.ModifiedOnUtc = utcNow;
                }
            }

            if (entry.State == EntityState.Deleted && entry.Entity is ISoftDeleteEntity softDeleteEntity)
            {
                entry.State = EntityState.Modified;
                softDeleteEntity.IsDeleted = true;
                softDeleteEntity.DeletedOnUtc = utcNow;

                if (entry.Entity is IAuditableEntity auditableOnDelete)
                {
                    auditableOnDelete.ModifiedOnUtc = utcNow;
                }
            }
        }
    }

    private static void ApplySoftDeleteQueryFilters(ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (!typeof(ISoftDeleteEntity).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            var method = typeof(TechDbContext)
                .GetMethod(nameof(GetSoftDeleteFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                .MakeGenericMethod(entityType.ClrType);

            var filter = (LambdaExpression)method.Invoke(null, null)!;
            builder.Entity(entityType.ClrType).HasQueryFilter(filter);
        }
    }

    private static Expression<Func<TEntity, bool>> GetSoftDeleteFilter<TEntity>()
        where TEntity : class, ISoftDeleteEntity
    {
        return entity => !entity.IsDeleted;
    }
}
