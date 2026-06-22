using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using System.Text;
using TechStore.BLL;
using TechStore.BLL.Auth;
using TechStore.BLL.Mappings;
using TechStore.DAL.Context;
using TechStore.DAL.Extensions;
using TechStore.DAL.UnitOfWork;
using TechStore.Domain.Entities;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using TechStore.API.Middlewares;

namespace TechStore.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins(
                        builder.Configuration.GetSection("CorsOrigins").Get<string[]>() 
                        ?? new[] { "http://localhost:3000", "http://localhost:5173" })
                    .WithHeaders("Authorization", "Content-Type", "Accept")
                    .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS")
                    .AllowCredentials();
            });
        });

        builder.Services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("AuthLimiter", opt =>
            {
                opt.PermitLimit = 5;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 0;
            });
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly));
        builder.Services.AddDbContext<TechDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
        })
 .AddEntityFrameworkStores<TechDbContext>()
 .AddDefaultTokenProviders();

        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
        builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));
        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        builder.Services.AddScoped<IJwtService, JwtService>();
        builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        builder.Services.AddScoped<IAuthService, AuthService>();

        builder.Services.AddMemoryCache();
        builder.Services.AddDistributedMemoryCache();
        
        StripeConfiguration.ApiKey = builder.Configuration["StripeSettings:SecretKey"];
        
        builder.Services.AddBllServices();

        builder.Services.AddAuthorization();

        var app = builder.Build();

        app.UseMiddleware<ExceptionHandlerMiddleware>();
        app.UseRateLimiter();

        await app.SeedIdentityDataAsync();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseHttpsRedirection();
        app.UseCors("AllowFrontend");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
