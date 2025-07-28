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
        services.AddLoggingAndSerilog();
        services.AddDatabase(configuration);
        services.AddIdentityConfiguration();
        services.AddJwtAuthentication(configuration);
        services.AddApplicationLayerServices();
        services.AddProjectServices();
        services.AddLocalizationConfiguration();
        services.AddAuthorization();
    }

    public static Task ConfigureAsync(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        return app.ConfigureAppMiddlewareAsync(env);
    }
}