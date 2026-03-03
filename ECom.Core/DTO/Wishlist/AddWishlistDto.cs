
using System.ComponentModel.DataAnnotations;

namespace ECom.Core.DTO.Wishlist
{
    public record AddWishlistDto
    {
        [Required]
        public int ProductId { get; init; }
    }
}
