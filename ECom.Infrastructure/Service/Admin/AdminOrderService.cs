using AutoMapper;
using ECom.Application.DTO.Order;
using ECom.Core.Entities.Order;
using ECom.Application.Sharing;
using ECom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ECom.Application.Interfaces.Services.Admin;
namespace ECom.Infrastructure.Service.Admin
{
    public class AdminOrderService : IAdminOrderService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AdminOrderService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IReadOnlyList<OrderToReturnDto>> GetAllOrdersAsync(PaymentStatus? status = null)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.DeliveryMethod)
                .AsNoTracking()
                .AsQueryable();

            // Filter by status if provided
            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return _mapper.Map<IReadOnlyList<OrderToReturnDto>>(orders);
        }
        public async Task<OrderToReturnDto> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.DeliveryMethod)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order is null) return null;

            return _mapper.Map<OrderToReturnDto>(order);
        }
        public async Task<ResponseAPI> UpdateOrderStatusAsync(int orderId, PaymentStatus status)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order is null)
                return new ResponseAPI(404, "Order not found");

            // Prevent invalid status transitions
            if (order.Status == PaymentStatus.Cancelled)
                return new ResponseAPI(400, "Cannot update a cancelled order");

            if (order.Status == status)
                return new ResponseAPI(400, $"Order is already {status}");

            order.Status = status;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return new ResponseAPI(200, $"Order status updated to {status} successfully");
        }
        public async Task<Dictionary<string, int>> GetOrdersCountByStatusAsync()
        {
            var counts = await _context.Orders
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                .ToListAsync();

            return counts.ToDictionary(x => x.Status, x => x.Count);
        }
    }
}
