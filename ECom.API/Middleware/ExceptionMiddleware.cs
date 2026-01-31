using ECom.API.Helper;
using System.Net;
using System.Text.Json;

namespace ECom.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _environment;

        public ExceptionMiddleware(RequestDelegate next, IHostEnvironment environment)
        {
            _next = next;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
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
    }
}
