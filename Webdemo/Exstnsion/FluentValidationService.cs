using FluentValidation.AspNetCore;
using FluentValidation;
using Interface.Model.ValidatorsModel;
namespace Webdemo.Exstnsion

{
    public static class FluentValidationService
    {
        public static IServiceCollection AddValidationServices(this IServiceCollection services)
        {
            // MVC
            services.AddControllersWithViews();

            // Register FluentValidation validators from this assembly
            services.AddValidatorsFromAssemblyContaining<CartItemModelValidator>();
            services.AddValidatorsFromAssemblyContaining<CategoryModelValidator>();
            // Enable automatic server-side validation
            services.AddFluentValidationAutoValidation();
            // Enable client-side adapters for Razor Pages/Views
            services.AddFluentValidationClientsideAdapters();

            return services;
        }
    }
}


