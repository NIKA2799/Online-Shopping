namespace Webdemo.Startup;
using Dto;
using Interface;
using Interface.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Repositories.Repositories;
using Serilog;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Webdemo.Exstnsion;

public static class Startup
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(new ConfigurationBuilder()
             .AddJsonFile("seri-log.config.json")
             .Build())
            .Enrich.FromLogContext()
             .CreateLogger();
        services.AddLogging();
        services.AddSerilog(logger);
        // Replace the problematic line with the following:
        services.AddDbContext<WebDemoDbContext>(options =>
          options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Password settings
            options.Password.RequireNonAlphanumeric = false;  // No special characters required
            options.Password.RequiredLength = 8;              // Minimum length of 8 characters
            options.Password.RequireUppercase = false;        // No uppercase letters required
            options.Password.RequireLowercase = false;        // No lowercase letters required

            // User settings
            options.User.RequireUniqueEmail = true;           // Ensure email is unique

            // Sign-in settings
            options.SignIn.RequireConfirmedAccount = false;   // No confirmed account required
            options.SignIn.RequireConfirmedEmail = false;     // No confirmed email required
            options.SignIn.RequireConfirmedPhoneNumber = false; // No confirmed phone number required
        })
        .AddEntityFrameworkStores<WebDemoDbContext>()    // Use EF Core for Identity
        .AddDefaultTokenProviders();                         // Add default token providers
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
        });

        // Add controllers with views (MVC)
        services.AddControllersWithViews();
        services.AddProjectServices();
        // Add Razor Pages
        services.AddRazorPages();
        services.Configure<EmailSettings>(
        configuration.GetSection("EmailSettings"));
        // Add session support (optional)
        services.AddSession();
        services.AddLocalization(options => options.ResourcesPath = "Resources");
        var supportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("ka-GE") };
        // ✅ Configure RequestLocalization
        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture("ka-GE");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
            options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
            options.RequestCultureProviders.Insert(1, new CookieRequestCultureProvider());

        });
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
          .AddJwtBearer(options =>
          {
              var jwtKey = configuration["Jwt:Key"];
              if (string.IsNullOrEmpty(jwtKey))
              {
                  throw new ArgumentNullException(nameof(jwtKey), "Jwt:Key configuration value cannot be null or empty.");
              }

              options.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateIssuer = true,
                  ValidateAudience = true,
                  ValidateLifetime = true,
                  ValidateIssuerSigningKey = true,
                  ValidIssuer = configuration["Jwt:Issuer"],
                  ValidAudience = configuration["Jwt:Audience"],
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
              };
          });
        services.AddAuthorization();

    }
    // This method gets called by the runtime to configure the HTTP request pipeline.
    public static Task ConfigureAsync(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts(); // Enforce HTTP Strict Transport Security  
        }

        var locOptions = app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>();
        app.UseErrorHandling();
        app.UseRequestMiddleware();

        app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS  
        app.UseStaticFiles();      // Serve static files (CSS, JS, images)  
        app.UseCustomMiddleware();
        app.UseRouting();          // Enable routing middleware  

        app.UseAuthentication();   // Enable authentication middleware  
        app.UseAuthorization();    // Enable authorization middleware  

        app.UseSession();          // Enable session state  

        app.UseEndpoints(endpoints =>
        {
            // Default route for MVC  
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Razor Pages route  
            endpoints.MapRazorPages();
        });

        // Return a completed task to avoid CS1998 diagnostic  
        return Task.CompletedTask;
    }
}