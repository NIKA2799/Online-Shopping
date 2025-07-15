using Dto;
using Microsoft.AspNetCore.Identity;

namespace Webdemo.Exstnsion
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;

    public static class SeedAdminHelper
    {
        public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var sp = scope.ServiceProvider;

            // instead of ILogger<SeedAdminHelper>:
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("SeedAdminHelper");

            var config = sp.GetRequiredService<IConfiguration>();
            var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();

            // … the rest of your seeding logic
        

        // 1) Seed roles
        string[] roles = new[] { "admin", "user" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                    if (roleResult.Succeeded)
                        logger.LogInformation("Role '{Role}' created.", role);
                    else
                        logger.LogError("Failed to create role '{Role}': {Errors}", role, string.Join("; ", roleResult.Errors));
                }
            }

            // 2) Seed admin user from config
            var adminEmail = config["AdminSettings:Email"];
            var adminPassword = config["AdminSettings:Password"];

            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            {
                logger.LogWarning("AdminSettings:Email or Password is not configured. Skipping admin creation.");
                return;
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (!createResult.Succeeded)
                {
                    logger.LogError("Failed to create admin user '{Email}': {Errors}", adminEmail,
                        string.Join("; ", createResult.Errors));
                    return;
                }

                logger.LogInformation("Admin user '{Email}' created.", adminEmail);

                // 3) Assign admin role
                var roleAssign = await userManager.AddToRoleAsync(adminUser, "admin");
                if (roleAssign.Succeeded)
                    logger.LogInformation("Assigned 'admin' role to '{Email}'.", adminEmail);
                else
                    logger.LogError("Failed to assign 'admin' role to '{Email}': {Errors}", adminEmail,
                        string.Join("; ", roleAssign.Errors));
            }
            else
            {
                logger.LogInformation("Admin user '{Email}' already exists. Skipping.", adminEmail);
            }
        }
    }
}