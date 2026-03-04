
using AutoMapper;
using ECom.Core.DTO.Wishlist;
using ECom.Core.Entities.Product;
using ECom.Core.Interfaces;
using ECom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECom.Infrastructure.Repositories
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public WishlistRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public Task AddToWishlistAsync(Wishlist wishlist)
        {
            _context.Wishlists.Add(wishlist);
            return Task.CompletedTask;
        }

        public async Task<IReadOnlyList<WishlistDto>> GetUserWishlistAsync(string userId)
        {
            var wishList = await _context.Wishlists
                .AsNoTracking()
                .Where(w => w.UserId == userId)
                .Include(w => w.Product)
                    .ThenInclude(p => p.Photos)
                .OrderByDescending(w => w.AddedAt)
                .ToListAsync();

            return _mapper.Map<IReadOnlyList<WishlistDto>>(wishList);
        }

        public async Task<Wishlist> GetWishlistItemAsync(int productId, string userId)
        {
            return await _context.Wishlists
                .FirstOrDefaultAsync(w => w.ProductId == productId && w.UserId == userId);
        }

        public async Task<bool> IsProductInWishlistAsync(int productId, string userId)
        {
            return await _context.Wishlists
                .AnyAsync(w => w.ProductId == productId && w.UserId == userId);
        }

        public Task RemoveFromWishlistAsync(Wishlist wishlist)
        {
            _context.Wishlists.Remove(wishlist);
            return Task.CompletedTask;
        }
    }
}
