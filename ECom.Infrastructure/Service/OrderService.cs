using ECom.Core.DTO.Order;
using ECom.Core.Entities.Order;
using ECom.Core.Interfaces;
using ECom.Core.Services;
using ECom.Infrastructure.Data;


namespace ECom.Infrastructure.Service
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;
        public OrderService(IUnitOfWork unitOfWork, AppDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public Task<Orders> CreateOrder(OrderDto orderDto, string BuyerEmail)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Orders>> GetAllOrdersForUserAsync(string BuyerEmail)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<DeliveryMethod>> GetDealMethodAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Orders> GetOrderByIdAsync(int id, string BuyerEmail)
        {
            throw new NotImplementedException();
        }
    }
}
