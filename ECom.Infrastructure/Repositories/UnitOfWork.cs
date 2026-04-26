using AutoMapper;
using ECom.Core.Entities;
using ECom.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<AppUser> _userManager; // Inject UserManager for Identity
        private readonly IEmailService _emailService;
        private readonly SignInManager<AppUser> _signInManager; // Inject SignInManager for Identity
        private readonly IGenerateToken _generateToken; // Inject IGenerateToken for token generation   
        private readonly IConfiguration _configuration;

        public ICategoryRepository CategoryRepository { get; }

        public IProductRepository ProductRepository { get; }

        public IPhotoRepository PhotoRepository { get; }

        public ICustomerBasketRepository CustomerBasketRepository { get; }

        public IAuth AuthRepository { get; }

        public IReviewRepository ReviewRepository { get; }

        public IWishlistRepository WishlistRepository { get; }

        public IOrderRepository OrderRepository { get; }

        public IDeliveryMethodRepository DeliveryMethodRepository { get; }

        public UnitOfWork(AppDbContext context, IMapper mapper, IImageManagementService imageManagementService,
            IConnectionMultiplexer redis, UserManager<AppUser> userManager, IEmailService emailService,
            SignInManager<AppUser> signInManager, IGenerateToken generateToken, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _redis = redis;
            _userManager = userManager;
            _imageManagementService = imageManagementService;
            _emailService = emailService;
            _signInManager = signInManager;
            _generateToken = generateToken;
            _configuration = configuration;
            CategoryRepository = new CategoryRepository(context);
            ProductRepository = new ProductRepository(context, _mapper, _imageManagementService);
            PhotoRepository = new PhotoRepository(context); // Initialize PhotoRepository
            CustomerBasketRepository = new CustomerBasketRepository(redis); // Initialize CustomerBasketRepository in UnitOfWork constructor
            AuthRepository = new AuthRepository(userManager, _emailService, _signInManager, generateToken, _context, _configuration); // Initialize AuthRepository in UnitOfWork constructor
            ReviewRepository = new ReviewRepository(context, _mapper);
            WishlistRepository = new WishlistRepository(context, _mapper);
            OrderRepository = new OrderRepository(context);
            DeliveryMethodRepository = new DeliveryMethodRepository(context);
        }

        public async Task<int> SaveChangesAsync()
           => await _context.SaveChangesAsync();
    }
}
