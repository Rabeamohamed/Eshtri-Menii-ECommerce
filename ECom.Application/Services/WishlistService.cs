using AutoMapper;
using ECom.Application.DTO.Wishlist;
using ECom.Core.Entities;
using ECom.Core.Entities.Product;
using ECom.Application.Sharing;
using ECom.Application.Interfaces.Repositories;
using ECom.Application.Interfaces.Services;

namespace ECom.Application.Services
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
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(dto.ProductId);
            if (product is null)
            {
                return new ResponseAPI(404, "Product not found.");
            }

            if (!product.InStock)
            {
                return new ResponseAPI(400, "Product is out of stock.");
            }

            var alreadyExists = await _unitOfWork.WishlistRepository.IsProductInWishlistAsync(dto.ProductId, userId);
            if (alreadyExists)
            {
                return new ResponseAPI(400, "Product is already in wishlist.");
            }

            var wishlist = new Wishlist
            {
                ProductId = dto.ProductId,
                UserId = userId,
                AddedAt = DateTime.UtcNow
            };
            await _unitOfWork.WishlistRepository.AddToWishlistAsync(wishlist);

            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                return new ResponseAPI(400, "Failed to add to wishlist.");
            }

            return new ResponseAPI(201, "Product added to wishlist successfully.");
        }

        public async Task<ResponseAPI> RemoveFromWishlistAsync(int productId, string userId)
        {
            var wishlistItem = await _unitOfWork.WishlistRepository.GetWishlistItemAsync(productId, userId);
            if (wishlistItem is null)
            {
                return new ResponseAPI(404, "Wishlist item not found.");
            }

            if (wishlistItem.UserId != userId)
            {
                return new ResponseAPI(403, "You do not have permission to remove this item.");
            }

            await _unitOfWork.WishlistRepository.RemoveFromWishlistAsync(wishlistItem);

            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                return new ResponseAPI(400, "Failed to remove from wishlist.");
            }
            return new ResponseAPI(200, "Product removed from wishlist successfully");
        }

        public async Task<ResponseAPI> MoveToBasketAsync(int productId, string userId, string basketId)
        {
            var wishlistItem = await _unitOfWork.WishlistRepository.GetWishlistItemAsync(productId, userId);
            if (wishlistItem is null)
            {
                return new ResponseAPI(404, "Wishlist item not found.");
            }

            if (wishlistItem.UserId != userId)
            {
                return new ResponseAPI(403, "You do not have permission to remove this item.");
            }

            var product = await _unitOfWork.ProductRepository
                .GetByIdAsync(productId, p => p.Photos, p => p.Category);

            if (product is null)
            {
                return new ResponseAPI(404, "Product not found.");
            }

            if (!product.InStock)
            {
                return new ResponseAPI(400, "Product is out of stock.");
            }

            var basket = await _unitOfWork.CustomerBasketRepository
                .GetBasketAsync(basketId);

            if (basket is null)
            {
                return new ResponseAPI(404, "Basket not found.");
            }

            var existingItem = basket.BasketItems
                .FirstOrDefault(i => i.Id == productId);

            if (existingItem is not null)
            {
                existingItem.Quantity++;
            }
            else
            {
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

            var updated = await _unitOfWork.CustomerBasketRepository.UpdateBasketAsync(basket);
            if (updated is null)
                return new ResponseAPI(400, "Could not update basket.");

            await _unitOfWork.WishlistRepository.RemoveFromWishlistAsync(wishlistItem);
            await _unitOfWork.SaveChangesAsync();

            return new ResponseAPI(200, "Product moved to basket successfully.");
        }
    }
}
