using AutoMapper;
using ECom.Application.DTO.Order;
using ECom.Application.Interfaces.Repositories;
using ECom.Application.Interfaces.Services;

namespace ECom.Application.Services
{
    public class SellerOrderService : ISellerOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SellerOrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OrderToReturnDto?> GetMyOrderByIdAsync(string sellerId, int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(orderId);
            if (order is null) return null;

            // Filter order items to only those belonging to the seller
            var sellerItems = order.OrderItems.Where(i => i.SellerId == sellerId).ToList();
            if (!sellerItems.Any()) return null; // If no items belong to this seller, they can't see the order.

            // Mask the order items
            order.OrderItems = sellerItems;

            // Recalculate the subtotal for just the seller's items
            order.SubTotal = sellerItems.Sum(i => i.Price * i.Quantity);

            return _mapper.Map<OrderToReturnDto>(order);
        }

        public async Task<IReadOnlyList<OrderToReturnDto>> GetMyOrdersAsync(string sellerId)
        {
            var orders = await _unitOfWork.OrderRepository.GetOrdersForSellerAsync(sellerId);
            
            // For each order, we only want to expose the items that belong to the seller
            foreach (var order in orders)
            {
                var sellerItems = order.OrderItems.Where(i => i.SellerId == sellerId).ToList();
                order.OrderItems = sellerItems;
                order.SubTotal = sellerItems.Sum(i => i.Price * i.Quantity);
            }

            return _mapper.Map<IReadOnlyList<OrderToReturnDto>>(orders);
        }
    }
}
