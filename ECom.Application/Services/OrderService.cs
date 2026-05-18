using AutoMapper;
using ECom.Application.Common.Exceptions;
using ECom.Application.DTO.Order;
using ECom.Application.Interfaces.Repositories;
using ECom.Application.Interfaces.Services;
using ECom.Application.Sharing;
using ECom.Core.Entities.Order;
using ECom.Core.Entities.Product;
using ECom.Core.Entities;
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
            if (orderDto is null)
                throw new BusinessException("Order payload is required.");
            if (string.IsNullOrWhiteSpace(orderDto.BasketId))
                throw new BusinessException("Basket id is required.");
            if (string.IsNullOrWhiteSpace(buyerEmail))
                throw new BusinessException("Authenticated user email is required.");

            var basket = await _unitOfWork.CustomerBasketRepository
                .GetBasketAsync(orderDto.BasketId);

            if (basket is null)
                throw new NotFoundException("Basket not found");

            var products = new Dictionary<int, Product>();
            foreach (var item in basket.BasketItems)
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.Id);
                if (product is null)
                    throw new NotFoundException("Product not found");
                products[item.Id] = product;
            }

            foreach (var item in basket.BasketItems)
            {
                var product = products[item.Id];
                if (product.StockQuantity < item.Quantity)
                    throw new BusinessException(
                        $"'{product.Name}' only has {product.StockQuantity} items in stock");
            }

            var orderItems = basket.BasketItems.Select(item =>
            {
                var product = products[item.Id];
                return new OrderItems(
                    product.Id, item.Image,
                    product.Name, item.Price,
                    item.Quantity, product.SellerId);
            }).ToList();

            var deliveryMethod = await _unitOfWork.DeliveryMethodRepository
                .GetByIdAsync(orderDto.DeliveryMethodId);

            if (deliveryMethod is null)
                throw new NotFoundException("Delivery method not found");

            if (orderDto.ShippingAddress is null)
                throw new BusinessException("Shipping address is required.");

            var subTotal = orderItems.Sum(o => o.Price * o.Quantity);

            var discountAmount = 0m;
            if (!string.IsNullOrEmpty(basket.CouponCode))
            {
                discountAmount = basket.DiscountAmount > 0 ? basket.DiscountAmount : 0;
            }

            var finalTotal = subTotal - discountAmount;

            var shippingAddress = _mapper.Map<ShippingAddress>(orderDto.ShippingAddress);

            var existOrder = await _unitOfWork.OrderRepository
                .GetOrderByPaymentIntentIdAsync(basket.PaymentIntentId);

            if (existOrder is not null)
            {
                await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
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
                    await _unitOfWork.SaveChangesAsync();
                });

                await _paymentService.CreateOrUpdatePaymentAsync(
                    basket.Id, deliveryMethod.Id);
            }

            Orders order = null!;

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                order = new Orders(
                    buyerEmail, finalTotal, shippingAddress,
                    deliveryMethod, orderItems, basket.PaymentIntentId);

                await _unitOfWork.OrderRepository.AddOrderAsync(order);

                foreach (var item in basket.BasketItems)
                {
                    var product = products[item.Id];
                    product.StockQuantity -= item.Quantity;
                    await _unitOfWork.ProductRepository.UpdateAsync(product);
                }

                await _unitOfWork.SaveChangesAsync();

                if (!string.IsNullOrEmpty(basket.CouponCode) && discountAmount > 0)
                {
                    await _couponService.IncrementCouponUsageAsync(basket.CouponCode);
                }
            });

            await _unitOfWork.CustomerBasketRepository
                .DeleteBasketAsync(orderDto.BasketId);

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

                await _notificationService.SendToAdminsAsync(
                    "New Order Received! 📦",
                    $"New order #{order.Id} placed by {buyerEmail}",
                    NotificationType.NewOrder);
            }
            catch
            {
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

        public async Task<OrderToReturnDto?> GetOrderByIdAsync(int id, string buyerEmail)
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

            var wasPaid = order.Status == PaymentStatus.PaymentReceived;

            if (wasPaid)
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

            return new ResponseAPI(200, wasPaid
                ? "Order cancelled and refund initiated successfully"
                : "Order cancelled and stock restored successfully");
        }
    }
}
