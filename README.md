TechStore API

A full-featured e-commerce REST API built with ASP.NET Core 8.0 following 3-layird Architecture principles. Supports product management, shopping cart, order processing, Stripe payments, coupon system, wishlists, reviews, and more.


🏗️ Architecture


3-layird Architecture — API, BLL, DAL, Domain layers fully separated
Repository Pattern with Unit of Work
Result/Error pattern for consistent, predictable error handling
JWT Authentication with refresh token rotation
Role-based Authorization (Admin / Customer)
AutoMapper for DTO mapping



🔧 Tech Stack

LayerTechnologyFrameworkASP.NET Core 8.0ORMEntity Framework CoreAuthASP.NET Core Identity + JWTPaymentsStripe APIEmailGmail SMTPDatabaseSQL ServerCachingIMemoryCacheMappingAutoMapper


🚀 Getting Started

Prerequisites


.NET 8.0 SDK
SQL Server
Visual Studio 2022 or VS Code
Stripe account (for payments)
Gmail account with App Password (for emails)


Installation


Clone the repository


bashgit clone https://github.com/yourusername/techstore-api.git
cd techstore-api


Configure appsettings.json


Fill in your own values for the following sections (do not commit real secrets):

json{
  "ConnectionStrings": {
    "DefaultConnection": "your-sql-server-connection-string"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-min-32-chars",
    "Issuer": "TechStoreApi",
    "Audience": "TechStoreClient",
    "ExpiryInMinutes": 60
  },
  "AdminSettings": {
    "Email": "admin@yourdomain.com",
    "Password": "YourStrongPassword123!",
    "Name": "System Admin",
    "PhoneNumber": "your-phone"
  },
  "StripeSettings": {
    "PublishableKey": "pk_test_...",
    "SecretKey": "sk_test_...",
    "WebhookSecret": "whsec_...",
    "Currency": "EGP"
  },
  "EmailSettings": {
    "FromEmail": "yourapp@gmail.com",
    "AppPassword": "your-gmail-app-password"
  }
}

Apply database migrations


bashdotnet ef database update


Run the application


bashdotnet run

📡 API Endpoints
Auth
Products
Cart
Orders
Payments
User & Addresses
Brands — CRUD (Admin), public read
Categories — Tree structure, CRUD (Admin), public read
Coupons — Validate & apply discounts
Wishlist — Add/remove/clear products
Reviews — Create, approve (Admin), rate products
Notifications — Per-user, mark read, admin broadcast
Newsletter — Subscribe/unsubscribe
Homepage — Manage sections and featured items (Admin)



✅ Features

JWT authentication with refresh token rotation
Role-based authorization (Admin / Customer)
Password reset via email (Gmail SMTP)
Product variants, specs, and images
Shopping cart with stock validation
Order processing with database transaction safety
Stripe payment integration with webhook support
Coupon/discount system with usage limits
Address management with default address support
Wishlist functionality
Product reviews with admin moderation
In-memory caching with smart invalidation
Rate limiting on auth endpoints
Idempotency keys on critical operations (order placement, payments)
Global exception handling middleware
Consistent Result/Error pattern across all layers



