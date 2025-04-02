using System.Diagnostics;

namespace Webdemo.Exstnsion
{
    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomMiddleware> _logger;

        public CustomMiddleware(RequestDelegate next, ILogger<CustomMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.LogInformation($"Incoming Request: {context.Request.Method} {context.Request.Path}");

                await _next(context); // Call the next middleware in the pipeline

                stopwatch.Stop();
                _logger.LogInformation($"Request processed in {stopwatch.ElapsedMilliseconds} ms - Status Code: {context.Response.StatusCode}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, $"An error occurred while processing the request. Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
                context.Response.StatusCode = 500; // Internal Server Error
                await context.Response.WriteAsync("An unexpected error occurred.");
            }
        }
    }
}
