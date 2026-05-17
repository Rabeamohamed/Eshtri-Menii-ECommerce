using AutoMapper;
using ECom.Application.DTO.Product;
using ECom.Core.Entities.Product;
using ECom.Application.Sharing;
using ECom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ECom.Application.Interfaces.Repositories;
using ECom.Application.Interfaces.Services;

namespace ECom.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;
        private readonly IImageManagementService imageManagementService;

        public ProductRepository(AppDbContext context, IMapper mapper, IImageManagementService imageManagementService) : base(context)
        {
            this.context = context;
            this.mapper = mapper;
            this.imageManagementService = imageManagementService;
        }

        public async Task<int> CountAsync(ProductParams productParams)
        {
            var query = context.Products.AsNoTracking();
            query = ApplyFilters(query, productParams);
            return await query.CountAsync();
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync(ProductParams productParams)
        {
            var query = context.Products
                .Include(c => c.Category)
                .Include(p => p.Photos)
                .AsNoTracking();

            query = ApplyFilters(query, productParams);
            query = ApplySort(query, productParams.Sort);

            query = query
                .Skip((productParams.PageNumber - 1) * productParams.PageSize)
                .Take(productParams.PageSize);

            var products = await query.ToListAsync();
            return mapper.Map<List<ProductDto>>(products);
        }

        private static IQueryable<Product> ApplyFilters(IQueryable<Product> query, ProductParams productParams)
        {
            if (!string.IsNullOrWhiteSpace(productParams.Search))
            {
                var searchWords = productParams.Search
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                query = query.Where(p => searchWords.All(word =>
                    (p.Name ?? "").ToLower().Contains(word.ToLower()) ||
                    (p.Description ?? "").ToLower().Contains(word.ToLower())));
            }

            if (productParams.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == productParams.CategoryId.Value);

            return query;
        }

        private static IQueryable<Product> ApplySort(IQueryable<Product> query, string? sort)
        {
            var key = (sort ?? "").Trim();
            return key switch
            {
                "priceAsc" or "PriceAsc" => query.OrderBy(p => p.NewPrice),
                "priceDesc" or "PriceDesc" => query.OrderByDescending(p => p.NewPrice),
                "nameDesc" or "NameDesc" => query.OrderByDescending(p => p.Name),
                "nameAsc" or "NameAsc" => query.OrderBy(p => p.Name),
                _ => query.OrderBy(p => p.Name),
            };
        }

        public async Task<bool> AddAsync(AddProductDto productDto)
        {
            if (productDto is null) return false;

            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var product = mapper.Map<Product>(productDto);
                await context.Products.AddAsync(product);
                await context.SaveChangesAsync();

                if (productDto.Photo is { Count: > 0 })
                {
                    var imagePaths = await imageManagementService.AddImageAsync(productDto.Photo, productDto.Name);
                    var photo = imagePaths.Select(path => new Photo
                    {
                        ImageName = path,
                        ProductId = product.Id
                    }).ToList();

                    await context.Photos.AddRangeAsync(photo);
                    await context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> UpdateAsync(UpdateProductDto productDto)
        {
            if (productDto is null) return false;

            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var product = await context.Products
                    .Include(c => c.Category)
                    .Include(p => p.Photos)
                    .FirstOrDefaultAsync(x => x.Id == productDto.Id);

                if (product is null)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                mapper.Map(productDto, product);

                if (productDto.Photo is { Count: > 0 })
                {
                    var oldPhotos = await context.Photos.Where(p => p.ProductId == product.Id).ToListAsync();
                    foreach (var item in oldPhotos)
                        imageManagementService.DeleteImageAsync(item.ImageName);

                    context.Photos.RemoveRange(oldPhotos);

                    var imagePaths = await imageManagementService.AddImageAsync(productDto.Photo, productDto.Name);
                    var photos = imagePaths.Select(path => new Photo
                    {
                        ImageName = path,
                        ProductId = product.Id
                    }).ToList();

                    await context.Photos.AddRangeAsync(photos);
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task DeleteAsync(Product product)
        {
            var photos = context.Photos.Where(p => p.ProductId == product.Id).ToList();
            foreach (var item in photos)
                imageManagementService.DeleteImageAsync(item.ImageName);

            context.Remove(product);
            await context.SaveChangesAsync();
        }
    }
}
