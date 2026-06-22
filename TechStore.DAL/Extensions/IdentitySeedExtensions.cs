using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechStore.Domain.Entities;
using TechStore.Domain.Enums;

namespace TechStore.DAL.Extensions;

public static class IdentitySeedExtensions
{
    public static async Task SeedIdentityDataAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        foreach (var roleName in Enum.GetNames<UserRole>())
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(roleName));
            }
        }

        var adminSection = configuration.GetSection("AdminSettings");
        var adminEmail = adminSection["Email"];
        var adminPassword = adminSection["Password"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            return;
        }

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                Name = adminSection["Name"] ?? "Admin",
                PhoneNumber = adminSection["PhoneNumber"],
                EmailConfirmed = true,
                AccountStatus = AccountStatus.Active
            };

            var createResult = await userManager.CreateAsync(adminUser, adminPassword);

            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to create default admin user: {string.Join(", ", createResult.Errors.Select(x => x.Description))}");
            }
        }

        var adminRole = UserRole.Admin.ToString();

        if (!await userManager.IsInRoleAsync(adminUser, adminRole))
        {
            var addToRoleResult = await userManager.AddToRoleAsync(adminUser, adminRole);

            if (!addToRoleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to assign Admin role to default admin user: {string.Join(", ", addToRoleResult.Errors.Select(x => x.Description))}");
            }
        }
    }
}
