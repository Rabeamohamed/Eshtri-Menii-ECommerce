namespace ECom.Application.DTO.Order
{
    public record OrderItemsDto
    {
        public int ProductItemId { get; init; }
        public string MainImage { get; init; }
        public string ProductName { get; init; }
        public decimal Price { get; init; }
        public int Quantity { get; init; }
    }
}