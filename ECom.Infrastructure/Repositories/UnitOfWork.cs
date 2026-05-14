using AutoMapper;
using ECom.Core.Entities;
using ECom.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using ECom.Application.Interfaces.Repositories;
using ECom.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace ECom.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper; // Inject IMapper to send to ProductRepository
        private readonly IImageManagementService _imageManagementService; // Inject Image Management Service to send to ProductRepository
        private readonly IConnectionMultiplexer _redis; // Inject IConnectionMultiplexer for Redis
        private readonly IGenerateToken _generateToken; // Inject IGenerateToken for token generation   
        private readonly IConfiguration _configuration;

        public ICategoryRepository CategoryRepository { get; }

        public IProductRepository ProductRepository { get; }

        public IPhotoRepository PhotoRepository { get; }

        public ICustomerBasketRepository CustomerBasketRepository { get; }


        public IReviewRepository ReviewRepository { get; }

        public IWishlistRepository WishlistRepository { get; }

        public IOrderRepository OrderRepository { get; }

        public IDeliveryMethodRepository DeliveryMethodRepository { get; }
        public IAnalyticsRepository AnalyticsRepository { get; }
        public ICouponRepository CouponRepository { get; }

        public UnitOfWork(AppDbContext context, IMapper mapper, IImageManagementService imageManagementService,
            IConnectionMultiplexer redis, IGenerateToken generateToken, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _redis = redis;
            _imageManagementService = imageManagementService;
            _generateToken = generateToken;
            _configuration = configuration;
            CategoryRepository = new CategoryRepository(context);
            ProductRepository = new ProductRepository(context, _mapper, _imageManagementService);
            PhotoRepository = new PhotoRepository(context); // Initialize PhotoRepository
            CustomerBasketRepository = new CustomerBasketRepository(redis); // Initialize CustomerBasketRepository in UnitOfWork constructor
            ReviewRepository = new ReviewRepository(context, _mapper);
            WishlistRepository = new WishlistRepository(context, _mapper);
            OrderRepository = new OrderRepository(context);
            DeliveryMethodRepository = new DeliveryMethodRepository(context);
            AnalyticsRepository = new AnalyticsRepository(context);
            CouponRepository = new CouponRepository(context);
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
