using Microsoft.Extensions.DependencyInjection;
using TechStore.BLL.Interface;
using TechStore.BLL.Services;
using TechStore.BLL.Interfaces;
using TechStore.DAL.Generic;

namespace TechStore.BLL
{
    public static class BllServiceRegistration
    {
        public static IServiceCollection AddBllServices(this IServiceCollection services)
        {
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<IHomepageService, HomepageService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<INewsletterService, NewsletterService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IStripeService, StripeService>();
            services.AddScoped(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));


            return services;
        }
    }

}
