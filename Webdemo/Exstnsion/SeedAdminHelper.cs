using System;
using System.Linq;
using System.Threading.Tasks;
using Dto;
using Interface; // for ApplicationUser
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace  Webdemo.Exstnsion
{
    public static class SeedAdminHelper
    {

        public static async Task SeedAdminAsync(IServiceProvider sp)
        {
            // pull from DI
            // instead of GetRequiredService<ILogger<SeedAdminHelper>>
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("SeedAdminHelper");

            var userMgr = sp.GetRequiredService<UserManager<ApplicationUser>>();
            var roleMgr = sp.GetRequiredService<RoleManager<IdentityRole>>();
            var adminCfg = sp.GetRequiredService<IOptions<AdminSettings>>().Value;

            // 1) Create roles
            var roles = new[] { "admin", "user" };
            foreach (var role in roles)
            {
                if (!await roleMgr.RoleExistsAsync(role))
                {
                    var result = await roleMgr.CreateAsync(new IdentityRole(role));
                    if (result.Succeeded)
                        logger.LogInformation("Role '{Role}' created.", role);
                    else
                        logger.LogError("Failed to create role '{Role}': {Errors}",
                            role, string.Join(";", result.Errors.Select(e=>e.Description)));
                }
            }

            // 2) Validate secrets
            if (string.IsNullOrWhiteSpace(adminCfg.Email) ||
                string.IsNullOrWhiteSpace(adminCfg.Password))
            {
                logger.LogWarning("AdminSettings missing Email or Password; skipping seed.");
                return;
            }

            // 3) Seed the admin user
            var adminUser = await userMgr.FindByEmailAsync(adminCfg.Email);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser {
                    UserName       = adminCfg.Email,
                    Email          = adminCfg.Email,
                    EmailConfirmed = true
                };

                var create = await userMgr.CreateAsync(adminUser, adminCfg.Password);
                if (!create.Succeeded)
                {
                    logger.LogError("Failed to create admin '{Email}': {Errors}",
                        adminCfg.Email,
                        string.Join(";", create.Errors.Select(e=>e.Description)));
                    return;
                }

                logger.LogInformation("Admin user '{Email}' created.", adminCfg.Email);

                var addToRole = await userMgr.AddToRoleAsync(adminUser, "admin");
                if (addToRole.Succeeded)
                    logger.LogInformation("Assigned 'admin' role to '{Email}'.", adminCfg.Email);
                else
                    logger.LogError("Failed to assign 'admin' role to '{Email}': {Errors}",
                        adminCfg.Email,
                        string.Join(";", addToRole.Errors.Select(e=>e.Description)));
            }
            else
            {
                logger.LogInformation("Admin user '{Email}' already exists; skipping.", adminCfg.Email);
            }
        }
    }
}
