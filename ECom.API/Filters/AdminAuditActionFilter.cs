using Microsoft.AspNetCore.Mvc.Filters;

namespace ECom.API.Filters
{
    
    public sealed class AdminAuditActionFilter : IAsyncActionFilter
    {
        private readonly ILogger<AdminAuditActionFilter> _logger;

        public AdminAuditActionFilter(ILogger<AdminAuditActionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.User.IsInRole("Admin"))
            {
                await next();
                return;
            }

            var user = context.HttpContext.User.Identity?.Name
                       ?? context.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                       ?? "unknown";

            _logger.LogInformation(
                "Admin audit: {User} {Method} {Path}",
                user,
                context.HttpContext.Request.Method,
                context.HttpContext.Request.Path);

            await next();
        }
    }
}
