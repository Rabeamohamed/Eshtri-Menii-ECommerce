using System.ComponentModel.DataAnnotations;

namespace ECom.Core.DTO.Wishlist
{
    public record MoveToBasketDto
    {
        [Required]
        public int ProductId { get; init; }

        [Required]
        public string BasketId { get; init; } // Redis basket key
    }
}
