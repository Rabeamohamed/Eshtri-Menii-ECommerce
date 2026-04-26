using ECom.Application.Interfaces.Repositories;
using ECom.Core.Entities.Order;
using ECom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECom.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Orders> GetOrderByPaymentIntentIdAsync(string paymentIntentId)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o => o.PaymentIntentId == paymentIntentId);
        }

        public async Task<IReadOnlyList<Orders>> GetOrdersByEmailAsync(string buyerEmail)
        {
            return await _context.Orders
                .Where(o => o.BuyerEmail == buyerEmail)
                .Include(o => o.OrderItems)
                .Include(o => o.DeliveryMethod)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Orders> GetOrderByIdAsync(int id, string buyerEmail)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.DeliveryMethod)
                .Where(o => o.Id == id);

            //  If buyerEmail provided — filter by it (customer)
            // If null — admin can see any order
            if (!string.IsNullOrEmpty(buyerEmail))
                query = query.Where(o => o.BuyerEmail == buyerEmail);

            return await query.FirstOrDefaultAsync();
        }

        public Task AddOrderAsync(Orders order)
        {
            _context.Orders.Add(order);
            return Task.CompletedTask;
        }
        public Task UpdateOrderAsync(Orders order)
        {
            _context.Orders.Update(order);
            return Task.CompletedTask;
        }

        public Task DeleteOrderAsync(Orders order)
        {
            _context.Orders.Remove(order);
            return Task.CompletedTask;
        }
    }
}
