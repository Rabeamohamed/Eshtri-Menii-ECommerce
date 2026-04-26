
using System.ComponentModel.DataAnnotations;

namespace ECom.Application.DTO.Wishlist
{
    public record AddWishlistDto
    {
        [Required]
        public int ProductId { get; init; }
    }
}
