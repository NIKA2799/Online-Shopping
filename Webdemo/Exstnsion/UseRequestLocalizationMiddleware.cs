using Microsoft.Extensions.Options;

namespace Webdemo.Exstnsion
{
    public static class UseRequestLocalizationMiddleware
    {
        public static IApplicationBuilder UseRequestMiddleware(this IApplicationBuilder app)
        {
            var locOptions = app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>();
            return app.UseRequestLocalization(locOptions.Value);
        }
    }
}
