using Microsoft.AspNetCore.Mvc;
using ProductCatalog.API.Utility.Exception;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace ProductCatalog.API.Middleware
{
    [ExcludeFromCodeCoverage]
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Not found: {Message}", ex.Message);
                await WriteProblemAsync(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception processing {Method} {Path}",
                    context.Request.Method, context.Request.Path);
                await WriteProblemAsync(context, HttpStatusCode.InternalServerError,
                    "An unexpected error occurred.");
            }
        }

        private static async Task WriteProblemAsync(HttpContext context, HttpStatusCode status, string detail)
        {
            var problem = new ProblemDetails
            {
                Status = (int)status,
                Title = status == HttpStatusCode.NotFound ? "Resource not found" : "Server error",
                Detail = detail,
                Instance = context.Request.Path
            };

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = problem.Status.Value;
            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
