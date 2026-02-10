
namespace ECom.Core.Entities.Order
{
    public class Orders : BaseEntity<int>
    {
        public string BuyerEmail { get; set; }
        public decimal SubTotal { get; set; }
        public DateTime OrderDate { get; set; }= DateTime.Now;
        public ShippingAddress ShippingAddress { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public IReadOnlyList<OrderItems> OrderItems { get; set; }
        public Status PaymentStatus { get; set; }
    }

}
