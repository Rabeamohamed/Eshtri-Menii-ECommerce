using ECom.Core.Entities;
using ECom.Core.Interfaces;
using ECom.Core.Services;
using ECom.Infrastructure.Data;
using ECom.Infrastructure.Repositories;
using ECom.Infrastructure.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using StackExchange.Redis;
using System.Text;

namespace ECom.Infrastructure
{
    public static class InfrastructureRegistration
    {
        public static IServiceCollection InfrastructureConfiguration(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            service.AddHttpContextAccessor();

            // Applying Unit of Work Pattern
            service.AddScoped<IUnitOfWork, UnitOfWork>();

            // Registering Email Service
            service.AddScoped<IEmailService, EmailService>();

            //Register Order Service 
            service.AddScoped<IOrderService, OrderService>();

            // Registering Token
            service.AddScoped<IGenerateToken, GenerateToken>();

            //Registering Payment Service
            service.AddScoped<IPaymentService, PaymentService>();

            // Registering Review Service
            service.AddScoped<IReviewService, ReviewService>();

            // Apply Redis Connection for Caching
            service.AddSingleton<IConnectionMultiplexer>(i =>
            {
                var config = ConfigurationOptions.Parse(configuration.GetConnectionString("Redis"), true);
                return ConnectionMultiplexer.Connect(config);
            });


            // Register IFileProvider for ImageManagementService
            service.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            service.AddSingleton<IImageManagementService, ImageManagementService>();

            // Apply DbContext Registration
            service.AddDbContext<AppDbContext>(options => {
                options.UseSqlServer(configuration.GetConnectionString("EComConnection"));
            });

            // Apply Identity Configuration
            service.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            // Apply Authentication Configuration for JWT and Cookies Authentication 

            service.AddAuthentication(options =>
             {
                 options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                 options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                 options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
             })
                
                .AddCookie(o =>
             {
                 o.Cookie.Name = "token";
                 o.Events.OnRedirectToLogin = context =>
                 {
                     context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                     return Task.CompletedTask;
                 };
             })
             
             .AddJwtBearer(op=>
             {
                 op.RequireHttpsMetadata = false;
                 op.SaveToken = true;
                 op.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:Secret"])),
                     ValidateIssuer = true,
                     ValidIssuer = configuration["Token:Issuer"],
                     ValidateAudience = false,
                     ClockSkew = TimeSpan.Zero
                 };
                 op.Events = new JwtBearerEvents()
                 {
                     OnMessageReceived = context =>
                     {
                         var token = context.Request.Cookies["token"];
                         if (!string.IsNullOrEmpty(token))
                         {
                             context.Token = token;
                         }
                         return Task.CompletedTask;
                     }
                 };
             });

            return service;
        }
    }
}
