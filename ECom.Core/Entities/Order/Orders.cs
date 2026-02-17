
namespace ECom.Core.Entities.Order
{
    public class Orders : BaseEntity<int>
    {
        public Orders()
        {
            
        }
        public Orders(string buyerEmail, decimal subTotal, ShippingAddress shippingAddress, DeliveryMethod deliveryMethod, IReadOnlyList<OrderItems> orderItems)
        {
            BuyerEmail = buyerEmail;
            SubTotal = subTotal;
            ShippingAddress = shippingAddress;
            DeliveryMethod = deliveryMethod;
            OrderItems = orderItems;
            //PaymentIntentId = paymentIntentId;
        }

        public string BuyerEmail { get; set; }
        public decimal SubTotal { get; set; }
        public DateTime OrderDate { get; set; }= DateTime.Now;
        public ShippingAddress ShippingAddress { get; set; }
        public string PaymentIntentId { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public IReadOnlyList<OrderItems> OrderItems { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public decimal GetTotal()
        {
            return SubTotal + DeliveryMethod.Price;
        }
    }

}
