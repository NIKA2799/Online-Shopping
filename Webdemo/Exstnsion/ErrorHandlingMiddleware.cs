using System.Net;

namespace Webdemo.Exstnsion
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context); // Pass control to the next middleware
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, "Unhandled exception occurred while processing request for {Path}", context.Request.Path);

                // Return standard JSON error response
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var result = System.Text.Json.JsonSerializer.Serialize(new
                {
                    error = "An unexpected error occurred.",
                    detail = ex.Message // შესაძლებელია production-ზე არ გამოუტანო
                });

                await context.Response.WriteAsync(result);
            }
        }
    }
}