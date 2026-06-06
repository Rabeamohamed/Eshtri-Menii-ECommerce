using ECom.API.Filters;
using ECom.API.Middleware;
using ECom.API.Options;
using ECom.Application;
using ECom.Application.Interfaces.Services;
using ECom.Core.Entities;
using ECom.Infrastructure;
using ECom.Infrastructure.Data;
using ECom.Infrastructure.Hubs;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Serilog;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

        builder.Services.Configure<CookieAuthOptions>(
            builder.Configuration.GetSection(CookieAuthOptions.SectionName));

        var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                          ?? Array.Empty<string>();
        var corsPolicyName = builder.Configuration["Cors:PolicyName"] ?? "DefaultCors";

        builder.Services.AddCors(op =>
        {
            op.AddPolicy(corsPolicyName, policy =>
            {
                policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins(corsOrigins);
            });
        });

        builder.Services.AddMemoryCache();
        builder.Services.AddSignalR();
        //builder.Services.AddAuthentication().AddJwtBearer(options =>
        //    {
        //        options.Events = new JwtBearerEvents
        //        {
        //            OnMessageReceived = context =>
        //            {
        //                var accessToken = context.Request.Query["access_token"];
        //                var path = context.HttpContext.Request.Path;
        //                if (!string.IsNullOrEmpty(accessToken) &&
        //                    path.StartsWithSegments("/hub/notifications"))
        //                {
        //                    context.Token = accessToken;
        //                }
        //                return Task.CompletedTask;
        //            }
        //        };
        //    });

        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<AdminAuditActionFilter>();
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>("database");

        builder.Services.InfrastructureConfiguration(builder.Configuration);
        builder.Services.AddApplicationServices();  
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            SeedDevelopmentUsers(app);
        }

        app.UseSerilogRequestLogging();

        app.UseCors(corsPolicyName);

        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<ExceptionMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseStatusCodePagesWithReExecute("/errors/{0}");
        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/api/payment/webhook"))
                context.Request.EnableBuffering();
            await next();
        });

        app.MapHealthChecks("/health");
        app.MapControllers();
        app.MapHub<NotificationHub>("/hub/notifications");

        app.UseHangfireDashboard("/admin/hangfire", new DashboardOptions
        {
            Authorization = new[] { new HangfireAuthorizationFilter() }
        });

        RecurringJob.AddOrUpdate(
            "daily-sales-report",
            (IBackgroundJobService job) => job.SendDailySalesReportAsync(),
            Cron.Daily(23, 0)); // Every day at 11:00 PM

        RecurringJob.AddOrUpdate(
            "daily-cancelled-orders-cleanup",
            (IBackgroundJobService job) => job.CleanupCancelledOrdersAsync(),
            Cron.Daily(1, 0)); // Every day at 1:00 AM

        RecurringJob.AddOrUpdate(
            "daily-low-stock-alerts",
            (IBackgroundJobService job) => job.SendLowStockAlertsAsync(),
            Cron.Daily(8, 0)); // Every day at 8:00 AM

        app.Run();
    }
    private static void SeedDevelopmentUsers(WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                // Ensure roles exist
                var roles = new[] { "Admin", "Customer", "Vendor" };
                foreach (var role in roles)
                {
                    if (!roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                    {
                        roleManager.CreateAsync(new IdentityRole { Name = role, NormalizedName = role.ToUpper(), ConcurrencyStamp = Guid.NewGuid().ToString() }).GetAwaiter().GetResult();
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding/resetting development users.");
            }
        }
    }
}
