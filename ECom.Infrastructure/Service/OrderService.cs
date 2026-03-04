using AutoMapper;
using ECom.Core.DTO.Order;
using ECom.Core.Entities.Order;
using ECom.Core.Entities.Product;
using ECom.Core.Interfaces;
using ECom.Core.Services;
using ECom.Core.Sharing;
using ECom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;


namespace ECom.Infrastructure.Service
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPaymentService _paymentService;
        public OrderService(IUnitOfWork unitOfWork, AppDbContext context, IMapper mapper, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
            _paymentService = paymentService;
        }
        public async Task<Orders> CreateOrderAsync(OrderDto orderDto, string buyerEmail)
        {
            var basket = await _unitOfWork.CustomerBasketRepository.GetBasketAsync(orderDto.BasketId);

            // Fetch all products ONCE
            var products = new Dictionary<int, Product>();
            foreach (var item in basket.BasketItems)
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.Id);
                if (product is null)
                    throw new Exception("Product not found");
                products[item.Id] = product;
            }

            // 1 — Validate stock using cached products
            foreach (var item in basket.BasketItems)
            {
                var product = products[item.Id];
                if (product.StockQuantity < item.Quantity)
                    throw new Exception($"'{product.Name}' only has {product.StockQuantity} items in stock");
            }

            // 2 — Build order items using cached products
            var orderItems = basket.BasketItems.Select(item =>
            {
                var product = products[item.Id];
                return new OrderItems(product.Id, item.Image, product.Name, item.Price, item.Quantity);
            }).ToList();

            var deliveryMethod = await _context.DeliveryMethods
                .FirstOrDefaultAsync(d => d.Id == orderDto.DeliveryMethodId);

            var subTotal = orderItems.Sum(o => o.Price * o.Quantity);
            var shippingAddress = _mapper.Map<ShippingAddress>(orderDto.ShippingAddress);

            // 3 — Handle existing order
            var existOrder = await _context.Orders
                .Where(o => o.PaymentIntentId == basket.PaymentIntentId)
                .FirstOrDefaultAsync();

            if (existOrder is not null)
            {
                _context.Orders.Remove(existOrder);
                await _paymentService.CreateOrUpdatePaymentAsync(basket.PaymentIntentId, deliveryMethod.Id);
                await _context.SaveChangesAsync();
            }

            //  4 — Create order
            var order = new Orders(buyerEmail, subTotal, shippingAddress,
                deliveryMethod, orderItems, basket.PaymentIntentId);
            await _context.Orders.AddAsync(order);

            //5 — Decrement stock using cached products
            foreach (var item in basket.BasketItems)
            {
                var product = products[item.Id];
                product.StockQuantity -= item.Quantity;
                _context.Products.Update(product);
            }

            // Single SaveChanges — order + stock decrement in one transaction
            await _context.SaveChangesAsync();
            await _unitOfWork.CustomerBasketRepository.DeleteBasketAsync(orderDto.BasketId);
            return order;
        }

        public async Task<ResponseAPI> CancelOrderAsync(int orderId, string buyerEmail, bool isAdmin)
        {
            // 1. Get order with items
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order is null)
                return new ResponseAPI(404, "Order not found");

            // 2. Verify ownership — customer can only cancel their own orders
            if (!isAdmin && order.BuyerEmail != buyerEmail)
                return new ResponseAPI(403, "You are not allowed to cancel this order");

            // 3. Check cancellation rules based on status
            if (order.Status == PaymentStatus.PaymentFailed)
                return new ResponseAPI(400, "Failed orders cannot be cancelled");

            if (order.Status == PaymentStatus.Cancelled)
                return new ResponseAPI(400, "Order is already cancelled");

            // 4. Customer can only cancel Pending orders
            if (!isAdmin && order.Status == PaymentStatus.PaymentReceived)
                return new ResponseAPI(400, "Paid orders can only be cancelled by admin");

            // 5. Handle Stripe refund for paid orders
            if (order.Status == PaymentStatus.PaymentReceived)
            {
                var refundSuccess = await _paymentService.RefundPaymentAsync(order.PaymentIntentId);
                if (!refundSuccess)
                    return new ResponseAPI(400, "Failed to process refund — please try again");
            }

            // 6. Restore stock for each order item
            foreach (var item in order.OrderItems)
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductItemId);
                if (product is not null)
                {
                    product.StockQuantity += item.Quantity;
                    _context.Products.Update(product);
                }
            }

            // 7. Update order status
            order.Status = PaymentStatus.Cancelled;
            _context.Orders.Update(order);

            // 8. Save everything in one transaction
            await _context.SaveChangesAsync();

            return new ResponseAPI(200, order.Status == PaymentStatus.PaymentReceived
                ? "Order cancelled and refund initiated successfully"
                : "Order cancelled successfully");
        }
        public async Task<IReadOnlyList<OrderToReturnDto>> GetAllOrdersForUserAsync(string BuyerEmail)
        {
            var orders = await _context.Orders.Where( O=> O.BuyerEmail == BuyerEmail)
                .Include(OI => OI.OrderItems).Include(D => D.DeliveryMethod)
                .ToListAsync();
            var result = _mapper.Map<IReadOnlyList<OrderToReturnDto>>(orders);
            return result;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync()
        => await _context.DeliveryMethods.AsNoTracking().ToListAsync();

        public async Task<OrderToReturnDto> GetOrderByIdAsync(int id, string BuyerEmail)
        {
            var order = await _context.Orders.Where(O => O.Id == id && O.BuyerEmail == BuyerEmail)
                .Include(OI => OI.OrderItems).Include(D => D.DeliveryMethod)
                .FirstOrDefaultAsync();
            var result = _mapper.Map<OrderToReturnDto>(order);
            return result;
        }

    }
}
