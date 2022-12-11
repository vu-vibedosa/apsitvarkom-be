using System.Net;
using System.Text.Json;

namespace Apsitvarkom.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _environment;
        private readonly JsonSerializerOptions _options;

        public ExceptionHandlingMiddleware(RequestDelegate next, IWebHostEnvironment environment)
        {
            _next = next;
            _environment = environment;
            _options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorMessage = "An internal server error has occurred. If this error persists, please contact product's owners.";

            if (_environment.IsDevelopment())
            {
                var exception = new
                {
                    Message = ex.Message,
                    Source = ex.Source,
                    InnerExceptionMessage = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace
                };
                errorMessage = JsonSerializer.Serialize(exception, _options);
            }

            return context.Response.WriteAsync(errorMessage);
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
