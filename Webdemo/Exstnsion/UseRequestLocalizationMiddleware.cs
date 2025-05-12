using Microsoft.Extensions.Options;

namespace Webdemo.Exstnsion
{
    public static class UseRequestLocalizationMiddleware
    {
        public static IApplicationBuilder UseRequestMiddleware(this IApplicationBuilder app)
        {
           return app.UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
        }
    }
}


