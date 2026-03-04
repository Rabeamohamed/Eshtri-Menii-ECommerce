using AutoMapper;
using ECom.Core.DTO.Wishlist;
using ECom.Core.Entities;
using ECom.Core.Entities.Product;
using ECom.Core.Interfaces;
using ECom.Core.Services;
using ECom.Core.Sharing;

namespace ECom.Infrastructure.Service
{
    public class WishlistService : IWishlistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public WishlistService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<WishlistDto>> GetUserWishlistAsync(string userId)
        {
            return await _unitOfWork.WishlistRepository.GetUserWishlistAsync(userId);
        }
        public async Task<ResponseAPI> AddToWishlistAsync(AddWishlistDto dto, string userId)
        {
            // Check if product exists
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(dto.ProductId);
            if (product is null)
            {
                return new ResponseAPI(404,"Product not found.");
            }
            // Check if product is in stock
            if (!product.InStock)
            {
                return new ResponseAPI(400,"Product is out of stock.");
            }
            // Check if already in wishlist
                var alreadyExists = await _unitOfWork.WishlistRepository.IsProductInWishlistAsync(dto.ProductId,userId);
            if (!alreadyExists)
            {
                return new ResponseAPI(400,"Product is already in wishlist.");
            }
            // Add to wishlist
            var wishlist = new Wishlist
            {
                ProductId = dto.ProductId,
                UserId = userId,
                AddedAt = DateTime.UtcNow
            };
            await _unitOfWork.WishlistRepository.AddToWishlistAsync(wishlist);

            // Save changes
            var result = await _unitOfWork.SaveChangesAsync();
            if(result <= 0)
            {
                return new ResponseAPI(400, "ailed to add to wishlist.");
            }

            return new ResponseAPI(200, "Product added to wishlist successfully.");
        }


        public async Task<ResponseAPI> RemoveFromWishlistAsync(int productId, string userId)
        {
            // Get wishlist item
            var wishlistItem = await _unitOfWork.WishlistRepository.GetWishlistItemAsync(productId, userId);
            if(wishlistItem is null)
            {
                return new ResponseAPI(404,"Wishlist item not found.");
            }

            //  Verify ownership
            if(wishlistItem.UserId != userId)
            {
                return new ResponseAPI(403,"You do not have permission to remove this item.");
            }
            // Remove from wishlist
            await _unitOfWork.WishlistRepository.RemoveFromWishlistAsync(wishlistItem);

            // Save changes
            var result = await _unitOfWork.SaveChangesAsync();
            if(result <= 0)
            {
                return new ResponseAPI(400,"Failed to remove from wishlist.");
            }
            return new ResponseAPI(200, "Product removed from wishlist successfully");
        }
        public async Task<ResponseAPI> MoveToBasketAsync(int productId, string userId, string basketId)
        {
            // Get wishlist item
            var wishlistItem = await _unitOfWork.WishlistRepository.GetWishlistItemAsync(productId, userId);
            if(wishlistItem is null)
            {
                return new ResponseAPI(404,"Wishlist item not found.");
            }
            //  Verify ownership
            if (wishlistItem.UserId != userId)
            {
                return new ResponseAPI(403, "You do not have permission to remove this item.");
            }
            //  Get product details
            var product = await _unitOfWork.ProductRepository
                .GetByIdAsync(productId,p=>p.Photos, p => p.Category);

            if(product is null)
            {
                return new ResponseAPI(404,"Product not found.");
            }

            // Check stock before moving
            if (!product.InStock)
            {
                return new ResponseAPI(400,"Product is out of stock.");
            }
            // Get existing basket
            var basket = await _unitOfWork.CustomerBasketRepository
                .GetBasketAsync(basketId);

            if(basket is null)
            {
                return new ResponseAPI(404,"Basket not found.");
            }

            // Check if product already in basket

            var existingItem = basket.BasketItems
                .FirstOrDefault(i => i.Id == productId);

            if (existingItem is not null)
            {
                // Increment quantity if already in basket
                existingItem.Quantity++;
            }
            else
            {
                // Add new item to basket
                basket.BasketItems.Add(new BasketItem
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.NewPrice,
                    Quantity = 1,
                    Image = product.Photos?.FirstOrDefault()?.ImageName ?? "",
                    Category = product.Category?.Name ?? ""
                });
            }

            // 6. Update basket in Redis
            await _unitOfWork.CustomerBasketRepository.UpdateBasketAsync(basket);

            // 7. Remove from wishlist
            await _unitOfWork.WishlistRepository.RemoveFromWishlistAsync(wishlistItem);
            await _unitOfWork.SaveChangesAsync();

            return new ResponseAPI(200, "Product moved to basket successfully.");
        }
    }
}
