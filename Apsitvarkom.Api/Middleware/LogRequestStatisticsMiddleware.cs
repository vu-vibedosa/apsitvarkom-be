using System.Diagnostics;

namespace Apsitvarkom.Api.Middleware
{
    public class LogRequestStatisticsMiddleware
    {
        private readonly RequestDelegate _next;
        private int _currentRequest = 0;

        public LogRequestStatisticsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestId = Interlocked.Increment(ref _currentRequest);
            var sw = Stopwatch.StartNew();
            Console.WriteLine($"[{requestId}] [{context.Request.Method}] Request URL: {Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(context.Request)}");

            await _next(context);

            Console.WriteLine($"[{requestId}] Finished with status code {context.Response.StatusCode}. The request took {sw.ElapsedMilliseconds} ms.");
        }
    }

    public static class LogRequestStatisticsMiddlewareExtensions
    {
        public static IApplicationBuilder UseLogRequestStatistics(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LogRequestStatisticsMiddleware>();
        }
    }
}
