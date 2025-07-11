using System.Diagnostics;

namespace Webdemo.Exstnsion
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomMiddleware> _logger;

        // Constructor injects the next middleware and logger
        public CustomMiddleware(RequestDelegate next, ILogger<CustomMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // Core middleware logic: logs request info, measures time, handles errors
        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // Log the incoming HTTP request
                _logger.LogInformation("Incoming Request: {Method} {Path}", context.Request.Method, context.Request.Path);

                await _next(context); // Pass control to the next middleware

                stopwatch.Stop();
                // Log request duration and response code
                _logger.LogInformation(
                    "Request processed in {Elapsed} ms - Status Code: {StatusCode}",
                    stopwatch.ElapsedMilliseconds, context.Response.StatusCode
                );
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                // Log error details and elapsed time
                _logger.LogError(ex, "An error occurred while processing the request. Elapsed time: {Elapsed} ms", stopwatch.ElapsedMilliseconds);

                // Set a generic 500 response and return simple error message
                context.Response.StatusCode = 500;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("An unexpected error occurred.");
            }
        }
    }
}