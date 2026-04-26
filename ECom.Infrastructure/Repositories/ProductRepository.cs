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

        public async Task<IEnumerable<ProductDto>> GetAllAsync(ProductParams productParams)
        {
            var query = context.Products   // Use IQueryable for deferred execution because IQueryable is faster than IEnumerable 
                .Include(C => C.Category)           // because it translates queries to SQL and executes them on the database server not in your machine
                .Include(P => P.Photos)
                .AsNoTracking();  // AsNoTracking improves performance for read-only scenarios not see any changes of updates give better performance

            // Apply search filter by word on Name and Description if search parameter is provided
            if(!string.IsNullOrEmpty(productParams.Search))
            {
                var searchWords = productParams.Search.Split(" ");

                query = query.Where(p => searchWords.All(word =>

                p.Name.ToLower().Contains(word.ToLower()) ||
                p.Description.ToLower().Contains(word.ToLower())
                
                ));
            }

            // Apply category filter if categoryId is provided
            if (productParams.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == productParams.CategoryId.Value);
            }

            // Apply sorting based on the sort parameter

            query = productParams.Sort switch
            {
                "PriceAce" => query.OrderBy(p => p.NewPrice),
                "PriceDce" => query.OrderByDescending(p => p.NewPrice),
                "NameDce" => query.OrderByDescending(p => p.Name),
                _ => query.OrderBy(p => p.Name),
            };
           

            // Apply pagination , Pagination is the last step after filtering and sorting or any other operation it is the final operation

            query = query.Skip((productParams.PageNumber - 1) * productParams.PageSize).Take(productParams.PageSize); // Skip the records of previous pages and take only the records of the current page

            //var products = await query.ToListAsync();

            var result = mapper.Map<List<ProductDto>>(query);
            return result;

        }

        public async Task<bool> AddAsync(AddProductDto productDto)
        {
            if (productDto is null)return false;

            var product = mapper.Map<Product>(productDto);
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            var  ImagePaths = await imageManagementService.AddImageAsync(productDto.Photo, productDto.Name);

            var photo = ImagePaths.Select(path => new Photo
            {
                ImageName = path,
                ProductId = product.Id
            }).ToList();

            await context.Photos.AddRangeAsync(photo);
            await context.SaveChangesAsync();
            return true;

        }

        public async Task<bool> UpdateAsync(UpdateProductDto productDto)
        {
            if (productDto is null) return false;

            var product = await context.Products.Include(c=>c.Category)
                .Include(p=>p.Photos).FirstOrDefaultAsync(x => x.Id == productDto.Id);

            if (product is null) return false;

            mapper.Map(productDto, product);

            var oldPhotos = await context.Photos.Where(p => p.ProductId == product.Id).ToListAsync(); // Get existing photos before updating

            foreach (var item in oldPhotos)
            {
                imageManagementService.DeleteImageAsync(item.ImageName);
            }
            context.Photos.RemoveRange(oldPhotos);

            var ImagePaths = await imageManagementService.AddImageAsync(productDto.Photo, productDto.Name);
            var photos = ImagePaths.Select(path => new Photo
            {
                ImageName = path,
                ProductId = product.Id
            }).ToList();

            await context.Photos.AddRangeAsync(photos);
            //context.Products.Update(product);
            await context.SaveChangesAsync();
            return true;

        }

        public async Task DeleteAsync(Product product)
        {
            var photos = context.Photos.Where(p => p.ProductId == product.Id).ToList();
            foreach (var item in photos)
            {
                imageManagementService.DeleteImageAsync(item.ImageName);
            }
            context.Remove(product);
            await context.SaveChangesAsync();
        }
    }
}
