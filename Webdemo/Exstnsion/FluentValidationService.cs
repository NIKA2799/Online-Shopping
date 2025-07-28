using FluentValidation.AspNetCore;
using FluentValidation;
using Interface.Model.ValidatorsModel;
namespace Webdemo.Exstnsion

{
    public static class FluentValidationService
    {
        public static IServiceCollection AddValidationServices(this IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddValidatorsFromAssemblyContaining<CartItemModelValidator>();
            services.AddValidatorsFromAssemblyContaining<CategoryModelValidator>();
            services.AddValidatorsFromAssemblyContaining<CheckoutModelValidator>();
            services.AddValidatorsFromAssemblyContaining<OrderDetailModelValidator>();
            services.AddValidatorsFromAssemblyContaining<OrderModelValidator>();
            services.AddValidatorsFromAssemblyContaining<ProductModelValidator>();
            services.AddValidatorsFromAssemblyContaining<OrderTrackingModelValidator>();
            services.AddValidatorsFromAssemblyContaining<EmailSettingsValidator>();
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            return services;
        }
    }
}


