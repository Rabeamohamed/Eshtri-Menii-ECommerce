using ECom.Application.Interfaces.Services;
using ECom.Application.Interfaces.Services.Admin;
using ECom.Application.Services;
using ECom.Application.Services.Admin;
using Microsoft.Extensions.DependencyInjection;

namespace ECom.Application
{
    public static class ApplicationRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IWishlistService, WishlistService>();
            
            // Admin Services
            services.AddScoped<IAdminCategoryService, AdminCategoryService>();
            services.AddScoped<IAdminProductService, AdminProductService>();
            services.AddScoped<IAdminOrderService, AdminOrderService>();
            services.AddScoped<IAdminAnalyticsService, AdminAnalyticsService>();

            return services;
        }
    }
}
