namespace TechStore.Domain.Routing;

public static class ApiRoutes
{
    public const string Root = "api";
    public const string Version = "v1";
    public const string Base = $"{Root}/{Version}";

    public static class Auth
    {
        public const string Prefix = $"{Base}/auth";
        public const string Register = "register";
        public const string Login = "login";
        public const string RefreshToken = "refresh-token";
        public const string Logout = "logout";
        public const string ForgotPassword = "forgot-password";
        public const string VerifyResetCode = "verify-reset-code";
        public const string ResetPassword = "reset-password";
        public const string ChangePassword = "change-password";
    }

    public static class Admin
    {
        public const string Prefix = $"{Base}/admin";
        public const string Users = "users";
        public const string UserById = "users/{userId:int}";
        public const string CreateUser = "users";
        public const string UpdateUser = "users/{userId:int}";
        public const string AssignRole = "users/assign-role";
        public const string ActivateUser = "users/{userId:int}/activate";
        public const string DeactivateUser = "users/{userId:int}/deactivate";
    }

    public static class User
    {
        public const string Prefix = $"{Base}/user";
        public const string Profile = "profile";
        public const string UpdateProfile = "profile";
        public const string Addresses = "addresses";
        public const string AddressById = "addresses/{addressId:int}";
        public const string SetDefaultAddress = "addresses/set-default";
    }
    public static class Product
    {
        public const string Prefix = $"{Base}/products";
        public const string GetById = "{id:int}";
        public const string GetBySlug = "slug/{slug}";
    }

    public static class Category
    {
        public const string Prefix = $"{Base}/categories";
        public const string GetTree = "tree";
        public const string GetActive = "active";
        public const string GetRoots = "roots";
        public const string GetChildren = "{parentId:int}/children";
        public const string GetById = "{id:int}";
        public const string GetBySlug = "slug/{slug}";
    }

    public static class Brand
    {
        public const string Prefix = $"{Base}/brands";
        public const string GetActive = "active";
        public const string GetById = "{id:int}";
        public const string GetBySlug = "slug/{slug}";
    }

    public static class Cart
    {
        public const string Prefix = $"{Base}/cart";
        public const string Items = "items";
        public const string ItemById = "items/{itemId:int}";
        public const string Clear = "";
    }

    public static class Order
    {
        public const string Prefix = $"{Base}/orders";
        public const string GetById = "{id:int}";
        public const string Cancel = "{id:int}/cancel";
    }

    public static class Payment
    {
        public const string Prefix = $"{Base}/payments";
        public const string UpdateStatus = "{orderId:int}/status";
    }

    public static class Review
    {
        public const string Prefix = $"{Base}/reviews";
        public const string GetById = "{id:int}";
        public const string ProductReviews = "product/{productId:int}";
        public const string MyReviews = "me";
        public const string Summary = "product/{productId:int}/summary";
    }

    public static class Wishlist
    {
        public const string Prefix = $"{Base}/wishlists";
        public const string Product = "{productId:int}";
        public const string Clear = "clear";
        public const string Count = "count";
    }

    public static class Coupon
    {
        public const string Prefix = $"{Base}/coupons";
        public const string GetById = "{id:int}";
        public const string GetByCode = "code/{code}";
        public const string Validate = "validate";
    }

    public static class Homepage
    {
        public const string Prefix = $"{Base}/homepage";
        public const string Setup = "setup";
        public const string Sections = "sections";
        public const string SectionById = "sections/{id:int}";
        public const string SectionItems = "sections/{sectionId:int}/items";
        public const string SectionItemById = "sections/{sectionId:int}/items/{itemId:int}";
    }

    public static class Newsletter
    {
        public const string Prefix = $"{Base}/newsletter";
        public const string Subscribe = "subscribe";
        public const string Unsubscribe = "unsubscribe";
        public const string Check = "check";
        public const string Delete = "{id:int}";
    }

    public static class Notification
    {
        public const string Prefix = $"{Base}/notifications";
        public const string UnreadCount = "unread/count";
        public const string MarkRead = "{id:int}/read";
        public const string MarkAllRead = "read-all";
        public const string SendToAll = "send-all";
        public const string Delete = "{id:int}";
    }
}

