using Interface;
using Interface.Command;
using Interface.IRepositories;
using Interface.Queries;
using Microsoft.AspNetCore.Identity;
using Repositories.Repositories;
using Service;
using Service.CommandService;
using Service.QueriesService;

namespace Webdemo.Exstnsion
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderCommandService, OrderCommand>();
            services.AddScoped<ICategoryCommand, CategoryCommand>();
            services.AddScoped<ICategoryQurey, CategoryQurey>();
            services.AddScoped<IOrderQurey, OrderQuery>();
            services.AddScoped<IProductCommand, ProductCommandService>();
            services.AddScoped<IProductQuery, ProductQueriesService>();
            services.AddScoped<IReviewQureyService, ReviewQueryService>();
            services.AddScoped<IReviewCommandService, ReviewCommandService>();
            services.AddScoped<IWhishlistService, WhishlistService>();
            services.AddScoped<IShippingService, ShippingService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailConfiguration, EmailService>();
            services.AddScoped<IDiscountService, DiscountService>();
            return services;
        }
    }
}