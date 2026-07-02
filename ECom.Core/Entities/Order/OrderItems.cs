namespace ECom.Core.Entities.Order
{
    public class OrderItems : BaseEntity<int>
    {
        public OrderItems()
        {

        }
        public OrderItems(int productItemId, string mainImage, string productName, decimal price, int quantity, string? sellerId = null)
        {
            ProductItemId = productItemId;
            MainImage = mainImage;
            ProductName = productName;
            Price = price;
            Quantity = quantity;
            SellerId = sellerId;
        }

        public int ProductItemId { get; set; }
        public string MainImage { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? SellerId { get; set; }
        public int OrderId { get; set; }
        public virtual Orders Order { get; set; }
    }
}