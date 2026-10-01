using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Data.Context;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Web.Data;

public static class AdminSeeder
{
    public static async Task SeedAsync(
        IServiceProvider serviceProvider)
    {
        using var scope =
            serviceProvider.CreateScope();

        var context =
            scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

        var passwordHasher =
            scope.ServiceProvider
                .GetRequiredService<PasswordHasher<User>>();

        var configuration =
            scope.ServiceProvider
                .GetRequiredService<IConfiguration>();

        var adminEmail =
            configuration["Admin:Email"];

        var adminPassword =
            configuration["Admin:Password"];

        if (string.IsNullOrWhiteSpace(adminEmail) ||
            string.IsNullOrWhiteSpace(adminPassword))
        {
            throw new InvalidOperationException(
                "Admin credentials are not configured.");
        }

        var adminExists =
            await context.Users
                .AnyAsync(user =>
                    user.Role == "Admin");

        if (adminExists)
            return;

        var admin = new User
        {
            Name = "System Admin",
            Email = adminEmail,
            Role = "Admin"
        };

        admin.PasswordHash =
            passwordHasher.HashPassword(
                admin,
                adminPassword);

        await context.Users.AddAsync(admin);

        await context.SaveChangesAsync();
    }
}