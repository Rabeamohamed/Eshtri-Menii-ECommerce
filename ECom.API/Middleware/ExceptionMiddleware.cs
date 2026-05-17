using ECom.API.Helper;
using ECom.Application.Common.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Text.Json;

namespace ECom.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _environment;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly TimeSpan _rateLimitWindow = TimeSpan.FromSeconds(30);

        public ExceptionMiddleware(
            RequestDelegate next,
            IHostEnvironment environment,
            IMemoryCache memoryCache,
            ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _environment = environment;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                ApplySecurity(context);

                if (IsRequestAllowed(context) == false)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    context.Response.ContentType = "application/json";

                    var response = new ApiExceptions(
                        (int)HttpStatusCode.TooManyRequests, "Too many requests. Please try again later.");

                    var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                    await context.Response.WriteAsJsonAsync(response, options);
                    return;
                }

                await _next(context);
            }
            catch (BusinessException bex)
            {
                _logger.LogWarning(bex, "Business rule violation");
                context.Response.StatusCode = bex.StatusCode;
                context.Response.ContentType = "application/json";
                var response = new ApiExceptions(bex.StatusCode, bex.Message);
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                await context.Response.WriteAsJsonAsync(response, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = _environment.IsDevelopment()
                    ? new ApiExceptions(context.Response.StatusCode, ex.Message, ex.StackTrace)
                    : new ApiExceptions(context.Response.StatusCode, "An unexpected error occurred.");
                
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }

        public bool IsRequestAllowed(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var cacheKey = $"Rate:{ip}";
            var dateNow = DateTime.Now;

            var (timestamp, count) = _memoryCache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _rateLimitWindow;
                return (timestamp: dateNow, count: 0);
            });

            if (dateNow - timestamp < _rateLimitWindow)
            {
                if (count >= 8)
                {
                    return false;
                }

                _memoryCache.Set(cacheKey, (timestamp, count + 1), _rateLimitWindow);
                return true;
            }

            _memoryCache.Set(cacheKey, (dateNow, 1), _rateLimitWindow);
            return true;
        }

        public void ApplySecurity(HttpContext context)
        {
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        }
    }
}
