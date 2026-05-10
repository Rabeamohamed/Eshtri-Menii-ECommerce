using AutoMapper;
using ECom.Application.DTO.Order;
using ECom.Application.Interfaces.Repositories;
using ECom.Application.Interfaces.Services;
using ECom.Application.Interfaces.Services.Admin;
using ECom.Application.Sharing;
using ECom.Core.Entities.Order;
using ECom.Core.Entities.Product;
using ECom.Core.Entities;
using ECom.Application.DTO.Coupon;
using Microsoft.AspNetCore.Identity;
using ECom.Core.Enums;

namespace ECom.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaymentService _paymentService;
        private readonly INotificationService _notificationService;
        private readonly ICouponService _couponService;
        private readonly UserManager<AppUser> _userManager;

        public OrderService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IPaymentService paymentService,
            INotificationService notificationService,
            ICouponService couponService,
            UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paymentService = paymentService;
            _notificationService = notificationService;
            _couponService = couponService;
            _userManager = userManager;
        }

        public async Task<Orders> CreateOrderAsync(OrderDto orderDto, string buyerEmail)
        {
            var basket = await _unitOfWork.CustomerBasketRepository
                .GetBasketAsync(orderDto.BasketId);

            if (basket is null)
                throw new Exception("Basket not found");

            var products = new Dictionary<int, Product>();
            foreach (var item in basket.BasketItems)
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.Id);
                if (product is null)
                    throw new Exception($"Product not found");
                products[item.Id] = product;
            }

            foreach (var item in basket.BasketItems)
            {
                var product = products[item.Id];
                if (product.StockQuantity < item.Quantity)
                    throw new Exception(
                        $"'{product.Name}' only has {product.StockQuantity} items in stock");
            }

            var orderItems = basket.BasketItems.Select(item =>
            {
                var product = products[item.Id];
                return new OrderItems(
                    product.Id, item.Image,
                    product.Name, item.Price,
                    item.Quantity);
            }).ToList();

            var deliveryMethod = await _unitOfWork.DeliveryMethodRepository
                .GetByIdAsync(orderDto.DeliveryMethodId);

            if (deliveryMethod is null)
                throw new Exception("Delivery method not found");

            // Calculate subtotal (calculate once)
            var subTotal = orderItems.Sum(o => o.Price * o.Quantity);

            // Apply coupon discount if exists
            var discountAmount = 0m;
            if (!string.IsNullOrEmpty(basket.CouponCode))
            {
                discountAmount = basket.DiscountAmount > 0 ? basket.DiscountAmount : 0;
            }

            // Calculate final total after discount
            var finalTotal = subTotal - discountAmount;

            var shippingAddress = _mapper.Map<ShippingAddress>(orderDto.ShippingAddress);

            var existOrder = await _unitOfWork.OrderRepository
                .GetOrderByPaymentIntentIdAsync(basket.PaymentIntentId);

            if (existOrder is not null)
            {
                // Restore stock from existing order before deleting it
                foreach (var item in existOrder.OrderItems)
                {
                    var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductItemId);
                    if (product is not null)
                    {
                        product.StockQuantity += item.Quantity;
                        await _unitOfWork.ProductRepository.UpdateAsync(product);
                    }
                }

                await _unitOfWork.OrderRepository.DeleteOrderAsync(existOrder);
                await _paymentService.CreateOrUpdatePaymentAsync(
                    basket.Id, deliveryMethod.Id);
                await _unitOfWork.SaveChangesAsync();
            }

            // Create order with final total (after discount)
            var order = new Orders(
                buyerEmail, finalTotal, shippingAddress,
                deliveryMethod, orderItems, basket.PaymentIntentId);

            await _unitOfWork.OrderRepository.AddOrderAsync(order);

            // Update product stock
            foreach (var item in basket.BasketItems)
            {
                var product = products[item.Id];
                product.StockQuantity -= item.Quantity;
                await _unitOfWork.ProductRepository.UpdateAsync(product);
            }

            await _unitOfWork.SaveChangesAsync();

            // Apply coupon and increment usage count if coupon was used
            if (!string.IsNullOrEmpty(basket.CouponCode) && discountAmount > 0)
            {
                await _couponService.IncrementCouponUsageAsync(basket.CouponCode);
            }

            // Delete basket after order is created
            await _unitOfWork.CustomerBasketRepository
                .DeleteBasketAsync(orderDto.BasketId);

            // Send Notifications
            try
            {
                var user = await _userManager.FindByEmailAsync(buyerEmail);
                if (user != null)
                {
                    await _notificationService.SendToUserAsync(
                        user.Id,
                        "Order Placed! 🛍️",
                        $"Your order #{order.Id} has been placed successfully.",
                        NotificationType.OrderPlaced);
                }

                // Notify admins
                await _notificationService.SendToAdminsAsync(
                    "New Order Received! 📦",
                    $"New order #{order.Id} placed by {buyerEmail}",
                    NotificationType.NewOrder);
            }
            catch
            {
                // Logic shouldn't fail if notification fails
            }

            return order;
        }

        public async Task<IReadOnlyList<OrderToReturnDto>> GetAllOrdersForUserAsync(
            string buyerEmail)
        {
            var orders = await _unitOfWork.OrderRepository
                .GetOrdersByEmailAsync(buyerEmail);
            return _mapper.Map<IReadOnlyList<OrderToReturnDto>>(orders);
        }

        public async Task<OrderToReturnDto> GetOrderByIdAsync(int id, string buyerEmail)
        {
            var order = await _unitOfWork.OrderRepository
                .GetOrderByIdAsync(id, buyerEmail);
            if (order is null) return null;
            return _mapper.Map<OrderToReturnDto>(order);
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync()
            => await _unitOfWork.DeliveryMethodRepository.GetAllAsync();

        public async Task<ResponseAPI> CancelOrderAsync(
            int orderId, string buyerEmail, bool isAdmin)
        {
            var order = await _unitOfWork.OrderRepository
                .GetOrderByIdAsync(orderId, isAdmin ? null : buyerEmail);

            if (order is null)
                return new ResponseAPI(404, "Order not found");

            if (!isAdmin && order.BuyerEmail != buyerEmail)
                return new ResponseAPI(403, "You are not allowed to cancel this order");

            if (order.Status == PaymentStatus.PaymentFailed)
                return new ResponseAPI(400, "Failed orders cannot be cancelled");

            if (order.Status == PaymentStatus.Cancelled)
                return new ResponseAPI(400, "Order is already cancelled");

            if (!isAdmin && order.Status == PaymentStatus.PaymentReceived)
                return new ResponseAPI(400, "Paid orders can only be cancelled by admin");

            if (order.Status == PaymentStatus.PaymentReceived)
            {
                var refundSuccess = await _paymentService
                    .RefundPaymentAsync(order.PaymentIntentId);
                if (!refundSuccess)
                    return new ResponseAPI(400, "Failed to process refund");
            }

            foreach (var item in order.OrderItems)
            {
                var product = await _unitOfWork.ProductRepository
                    .GetByIdAsync(item.ProductItemId);
                if (product is not null)
                {
                    product.StockQuantity += item.Quantity;
                    await _unitOfWork.ProductRepository.UpdateAsync(product);
                }
            }

            order.Status = PaymentStatus.Cancelled;
            await _unitOfWork.SaveChangesAsync();

            return new ResponseAPI(200, order.Status == PaymentStatus.PaymentReceived
                ? "Order cancelled and refund initiated successfully"
                : "Order cancelled and stock restored successfully");
        }
    }
}
