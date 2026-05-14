using System.Diagnostics;

namespace ECom.API.Middleware
{
    public class CorrelationIdMiddleware
    {
        private const string HeaderName = "X-Correlation-Id";
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILogger<CorrelationIdMiddleware> logger)
        {
            var correlationId = context.Request.Headers[HeaderName].FirstOrDefault()
                ?? Activity.Current?.Id
                ?? Guid.NewGuid().ToString("N");

            context.Response.Headers[HeaderName] = correlationId;
            context.Items["CorrelationId"] = correlationId;

            using (logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
            {
                await _next(context);
            }
        }
    }
}
