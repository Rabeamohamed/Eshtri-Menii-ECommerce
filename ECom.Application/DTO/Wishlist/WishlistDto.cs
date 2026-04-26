
namespace ECom.Application.DTO.Wishlist
{
    public record WishlistDto
    {
        public int Id { get; init; }
        public int ProductId { get; init; }
        public string ProductName { get; init; }
        public decimal Price { get; init; }
        public string MainPhoto { get; init; }
        public double AverageRating { get; init; }
        public bool InStock { get; init; }
        public DateTime AddedAt { get; init; }
    }
}
