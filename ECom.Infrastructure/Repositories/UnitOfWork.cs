using AutoMapper;
using ECom.Core.Entities;
using ECom.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using ECom.Application.Interfaces.Repositories;
using ECom.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECom.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IServiceProvider _serviceProvider;

        public ICategoryRepository CategoryRepository => _serviceProvider.GetRequiredService<ICategoryRepository>();
        public IProductRepository ProductRepository => _serviceProvider.GetRequiredService<IProductRepository>();
        public IPhotoRepository PhotoRepository => _serviceProvider.GetRequiredService<IPhotoRepository>();
        public ICustomerBasketRepository CustomerBasketRepository => _serviceProvider.GetRequiredService<ICustomerBasketRepository>();
        public IReviewRepository ReviewRepository => _serviceProvider.GetRequiredService<IReviewRepository>();
        public IWishlistRepository WishlistRepository => _serviceProvider.GetRequiredService<IWishlistRepository>();
        public IOrderRepository OrderRepository => _serviceProvider.GetRequiredService<IOrderRepository>();
        public IDeliveryMethodRepository DeliveryMethodRepository => _serviceProvider.GetRequiredService<IDeliveryMethodRepository>();
        public IAnalyticsRepository AnalyticsRepository => _serviceProvider.GetRequiredService<IAnalyticsRepository>();
        public ICouponRepository CouponRepository => _serviceProvider.GetRequiredService<ICouponRepository>();

        public UnitOfWork(AppDbContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
        }

        public async Task<int> SaveChangesAsync()
           => await _context.SaveChangesAsync();

        public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await action();
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
