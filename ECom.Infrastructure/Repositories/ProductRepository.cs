using AutoMapper;
using ECom.Core.DTO.Product;
using ECom.Core.Entities.Product;
using ECom.Core.Interfaces;
using ECom.Core.Services;
using ECom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
