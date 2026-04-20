using AutoMapper;
using ECom.API.Middleware;
using ECom.Infrastructure;
using Hangfire;
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);



        // CORS Policy Configuration 
        builder.Services.AddCors(op =>
        {
            op.AddPolicy("CROSPolicy", builder =>
            {
                builder.AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials()
                      .WithOrigins("http://localhost:4200"); // Adjust the origin as needed
            });
        });
        // In-Memory Caching Service Registration for Rate Limiting in Exception Middlewaret
        builder.Services.AddMemoryCache();

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.InfrastructureConfiguration(builder.Configuration);
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); // AutoMapper Configuration Registration Dependency Injection

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("CROSPolicy"); // CORS Middleware Registration

        app.UseMiddleware<ExceptionMiddleware>(); // Custom Exception Middleware Registration

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseStatusCodePagesWithReExecute("/errors/{0}"); // Global Error Handling Middleware Registration to Redirect to Error Controller

        app.UseHttpsRedirection();


        // Stripe Webhook Endpoint Middleware — must be before MapControllers to allow raw body reading for signature verification
        app.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/api/payment/webhook"))
                context.Request.EnableBuffering();
            await next();
        });
        app.MapControllers();
        
        // Add after app.UseAuthorization()
        app.UseHangfireDashboard("/admin/hangfire", new DashboardOptions
        {
            // Only admin can access dashboard
            Authorization = new[] { new HangfireAuthorizationFilter() }
        });

        app.Run();
    }
}