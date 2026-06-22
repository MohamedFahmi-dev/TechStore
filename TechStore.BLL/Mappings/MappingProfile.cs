using AutoMapper;
using TechStore.Domain.DTOs.Category;
using TechStore.Domain.DTOs.Product;
using TechStore.Domain.DTOs.Cart;
using TechStore.Domain.DTOs.User;
using TechStore.Domain.DTOs.Admin;
using TechStore.Domain.DTOs.Review;
using TechStore.Domain.DTOs.Notification;
using TechStore.Domain.DTOs.Newsletter;
using TechStore.Domain.DTOs.Coupon;
using TechStore.Domain.DTOs.Payment;
using TechStore.Domain.DTOs.Order;
using TechStore.Domain.DTOs.Homepage;
using TechStore.Domain.DTOs.Brand;
using TechStore.Domain.Entities;

namespace TechStore.BLL.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Category Maps
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ParentName, opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : null))
                .ForMember(dest => dest.Children, opt => opt.MapFrom(src => src.SubCategories))
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products != null ? src.Products.Count : 0));

            CreateMap<Brand, BrandDto>()
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products != null ? src.Products.Count : 0));

            // Product Variant Sub-Maps
            CreateMap<ProductImage, ProductImageDto>();
            CreateMap<ProductSpec, ProductSpecDto>();
            CreateMap<ProductVariantOptionValue, ProductVariantOptionValueDto>();
            CreateMap<ProductVariantOption, ProductVariantOptionDto>();
            CreateMap<ProductVariant, ProductVariantDto>();

            // Product Map (Summary)
            CreateMap<Product, ProductSummaryDto>()
                .ForMember(dest => dest.MainImageUrl, opt => opt.MapFrom(src => src.Images.FirstOrDefault(i => i.IsMain) != null ? src.Images.FirstOrDefault(i => i.IsMain).ImageUrl : null))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : string.Empty))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.Reviews != null && src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : 0))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews != null ? src.Reviews.Count : 0));

            // Product Map (Detail)
            CreateMap<Product, ProductDetailDto>()
                .IncludeBase<Product, ProductSummaryDto>()
                .ForMember(dest => dest.Specs, opt => opt.MapFrom(src => src.Specs.GroupBy(s => s.GroupName).Select(g => new ProductSpecGroupDto { GroupName = g.Key, Specs = g.Select(s => new ProductSpecDto { Id = s.Id, Name = s.Name, Value = s.Value, DisplayOrder = s.DisplayOrder }).ToList() })));

            // Cart Maps
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product != null && src.Product.Images != null ? src.Product.Images.FirstOrDefault(i => i.IsMain) != null ? src.Product.Images.FirstOrDefault(i => i.IsMain)!.ImageUrl : null : null))
                .ForMember(dest => dest.ProductSlug, opt => opt.MapFrom(src => src.Product != null ? src.Product.Slug : string.Empty))
                .ForMember(dest => dest.VariantLabel, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.Name : null))
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.StockQuantity : (src.Product != null ? src.Product.StockQuantity : 0)));

            CreateMap<Cart, CartDto>();

            // User Maps
            CreateMap<ApplicationUser, UserResponseDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName ?? string.Empty))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName ?? string.Empty))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email ?? string.Empty));
            CreateMap<ApplicationUser, UserProfileResponseDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName ?? string.Empty))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName ?? string.Empty))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email ?? string.Empty))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber ?? string.Empty));
            CreateMap<Address, AddressResponseDto>();

            // Review Maps
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Name : string.Empty))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedOnUtc));

            // Notification Maps
            CreateMap<Notification, NotificationDto>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedOnUtc));

            // Newsletter Maps
            CreateMap<NewsletterSubscriber, NewsletterSubscriberDto>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedOnUtc));

            // Coupon Maps
            CreateMap<Coupon, CouponDto>();

            // Payment Maps
            CreateMap<Payment, PaymentDto>();
            CreateMap<Payment, OrderPaymentDto>();

            // Order Maps
            CreateMap<OrderItem, OrderItemDto>();
            CreateMap<Address, OrderAddressDto>();
            CreateMap<Order, OrderSummaryDto>()
                .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.Items != null ? src.Items.Count : 0))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedOnUtc));
            CreateMap<Order, OrderDetailDto>()
                .IncludeBase<Order, OrderSummaryDto>()
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email ?? string.Empty : string.Empty))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => src.Payment))
                .ForMember(dest => dest.CouponCode, opt => opt.MapFrom(src => src.Coupon != null ? src.Coupon.Code : null));

            // Homepage Maps
            CreateMap<HomepageSectionItem, HomepageSectionItemDto>()
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product));
            CreateMap<HomepageSection, HomepageSectionDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
        }
    }
}
