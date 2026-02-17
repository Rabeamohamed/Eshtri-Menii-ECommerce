using AutoMapper;
using ECom.Core.DTO;
using ECom.Core.Entities;
using ECom.Core.Interfaces;
using ECom.Core.Services;
using ECom.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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



        public ICategoryRepository CategoryRepository { get; }

        public IProductRepository ProductRepository { get; }

        public IPhotoRepository PhotoRepository { get; }

        public ICustomerBasketRepository CustomerBasketRepository { get; }

        public IAuth AuthRepository { get; }

        public UnitOfWork(AppDbContext context, IMapper mapper, IImageManagementService imageManagementService
            , IConnectionMultiplexer redis, UserManager<AppUser> userManager, IEmailService emailService, SignInManager<AppUser> signInManager, IGenerateToken generateToken)
        {
            _context = context;
            _mapper = mapper;
            _redis = redis;
            _userManager = userManager;
            _imageManagementService = imageManagementService;
            _emailService = emailService;
            _signInManager = signInManager;
            _generateToken = generateToken;
            CategoryRepository = new CategoryRepository(context);
            ProductRepository = new ProductRepository(context, _mapper, _imageManagementService);
            PhotoRepository = new PhotoRepository(context); // Initialize PhotoRepository
            CustomerBasketRepository = new CustomerBasketRepository(redis); // Initialize CustomerBasketRepository in UnitOfWork constructor
            AuthRepository = new AuthRepository(userManager, _emailService, _signInManager,_generateToken, _context); // Initialize AuthRepository in UnitOfWork constructor
        }

    }
}
