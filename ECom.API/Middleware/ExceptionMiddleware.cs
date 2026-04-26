using ECom.API.Helper;
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
        private readonly TimeSpan _rateLimitWindow= TimeSpan.FromSeconds(30);

        public ExceptionMiddleware(RequestDelegate next, IHostEnvironment environment, IMemoryCache memoryCache)
        {
            _next = next;
            _environment = environment;
            _memoryCache = memoryCache;
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

                    await context.Response.WriteAsJsonAsync(response);
                }
                await _next(context);
            }
            catch (Exception ex)
            {

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = _environment.IsDevelopment()?
                    new ApiExceptions(context.Response.StatusCode, ex.Message,ex.StackTrace)
                        : new ApiExceptions(context.Response.StatusCode, ex.Message);
                var json = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(json);
            }
        }

        public bool IsRequestAllowed(HttpContext context)
        {
            var ip=context.Connection.RemoteIpAddress.ToString(); // Get client IP address of the request sender to apply rate limiting of sending requests
            var cacheKey = $"Rate:{ip}";
            var dateNow = DateTime.Now;

            // Try to get the existing record from the cache or create a new one if it doesn't exist
            // Create Tuple to hold timestamp and count of requests
            var (timestamp, count) = _memoryCache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _rateLimitWindow;
                return (timestamp : dateNow, count:0); // Initialize count to 0 and timestamp to current time if not found in cache
            });

            if(dateNow - timestamp < _rateLimitWindow)
            {
                if(count >= 8) // Limit to 5 requests in the defined time window
                {
                    return false; // Request not allowed
                }
                else
                {
                     // Increment request count
                    _memoryCache.Set(cacheKey, (timestamp, count+=1), _rateLimitWindow); // Update cache with new count
                    return true; // Request allowed
                }
            }
            else
            {
                // Time window has passed, reset count and timestamp
                _memoryCache.Set(cacheKey, (dateNow, count), _rateLimitWindow); // Reset count to 1 for the new request
            }
            return true; // Request allowed

        }

        public void ApplySecurity(HttpContext context)
        {
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff"); // Prevent MIME type sniffing apply content type options 
            context.Response.Headers.Add("X-Frame-Options", "DENY"); // Prevent Clickjacking attacks apply frame options
            context.Response.Headers.Add("X-XSS-Protection", "1; mode=block"); // Enable XSS protection in browsers apply XSS filtering protections
            //context.Response.Headers.Add("Referrer-Policy", "no-referrer");
            //context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self'");
        }

    }
}
