using System.ComponentModel.DataAnnotations;

namespace ECom.Application.DTO.Wishlist
{
    public record MoveToBasketDto
    {
        [Required]
        public int ProductId { get; init; }

        [Required]
        public string BasketId { get; init; } // Redis basket key
    }
}
