using Store.API.Helpers;
using System.Net;
using System.Text.Json;

namespace Store.API.Middleware
{
    // ==================================================
    // Global Error Handling Middleware
    // Catches ALL unhandled exceptions in the API
    // Returns clean ApiResponse instead of raw 500 error
    // ==================================================
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandlingMiddleware> _logger;

        public GlobalErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<GlobalErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Try to process the request normally
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, "An unhandled exception occurred");

                // Return clean error response
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = exception switch
            {
                InvalidOperationException => HttpStatusCode.BadRequest,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                KeyNotFoundException => HttpStatusCode.NotFound,
                ArgumentException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };

            var message = exception switch
            {
                InvalidOperationException => exception.Message,
                UnauthorizedAccessException => "Unauthorized access",
                KeyNotFoundException => exception.Message,
                ArgumentException => exception.Message,
                _ => "An unexpected error occurred. Please try again later."
            };

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                success = false,
                message = message,
                data = (object?)null
            };

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }

    // ==================================================
    // Extension method to register the middleware
    // ==================================================
    public static class GlobalErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalErrorHandling(
            this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalErrorHandlingMiddleware>();
        }
    }
}
