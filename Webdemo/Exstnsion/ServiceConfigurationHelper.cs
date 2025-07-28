using Dto;
using Microsoft.AspNetCore.Identity;
using Repositories.Repositories;

namespace Webdemo.Exstnsion
{
    public static class ServiceConfigurationHelper
    {
        public static void ConfigureIdentity(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                // User settings
                options.User.RequireUniqueEmail = true;

                // Sign-in settings
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;

            })
            .AddEntityFrameworkStores<WebDemoDbContext>()
            .AddDefaultTokenProviders();
        }

        public static void ConfigureCors(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
        }
    }
}