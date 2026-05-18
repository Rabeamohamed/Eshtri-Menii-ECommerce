using AutoMapper;
using ECom.Application.DTO.Order;
using ECom.Core.Entities.Order;
using ECom.Application.Sharing;
using ECom.Application.Interfaces.Repositories;
using ECom.Application.Interfaces.Services.Admin;

namespace ECom.Application.Services.Admin
{
    public class AdminOrderService : IAdminOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminOrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<OrderToReturnDto>> GetAllOrdersAsync(PaymentStatus? status = null)
        {
            var orders = await _unitOfWork.OrderRepository.GetAllOrdersAsync(status);
            return _mapper.Map<IReadOnlyList<OrderToReturnDto>>(orders);
        }

        public async Task<OrderToReturnDto?> GetOrderByIdAsync(int id)
        {
            var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(id);
            if (order is null) return null;
            return _mapper.Map<OrderToReturnDto>(order);
        }

        public async Task<ResponseAPI> UpdateOrderStatusAsync(int orderId, PaymentStatus status)
        {
            var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(orderId);
            if (order is null)
                return new ResponseAPI(404, "Order not found");

            if (order.Status == PaymentStatus.Cancelled)
                return new ResponseAPI(400, "Cannot update a cancelled order");

            if (order.Status == status)
                return new ResponseAPI(400, $"Order is already {status}");

            order.Status = status;
            await _unitOfWork.OrderRepository.UpdateOrderAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return new ResponseAPI(200, $"Order status updated to {status} successfully");
        }

        public async Task<Dictionary<string, int>> GetOrdersCountByStatusAsync()
        {
            return await _unitOfWork.OrderRepository.GetOrdersCountByStatusAsync();
        }
    }
}
