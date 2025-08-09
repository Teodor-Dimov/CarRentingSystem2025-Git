using CarRentingSystem2025.Models;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace CarRentingSystem2025.Middleware
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

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var response = new ErrorViewModel
            {
                RequestId = context.TraceIdentifier,
                ErrorMessage = "An error occurred while processing your request.",
                StatusCode = (int)HttpStatusCode.InternalServerError,
                OriginalPath = context.Request.Path
            };

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Don't expose sensitive information in production
            if (context.RequestServices.GetService<IWebHostEnvironment>()?.EnvironmentName == "Development")
            {
                response.ErrorDetails = exception.ToString();
                response.ErrorMessage = exception.Message;
            }

            await context.Response.WriteAsJsonAsync(response);
        }
    }

    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}



