using ECom.Application.Interfaces.Repositories;
using ECom.Core.Entities.Order;
using ECom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECom.Infrastructure.Repositories
{
    public class DeliveryMethodRepository : IDeliveryMethodRepository
    {
        private readonly AppDbContext _context;

        public DeliveryMethodRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DeliveryMethod> GetByIdAsync(int id)
        {
            return await _context.DeliveryMethods
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetAllAsync()
        {
            return await _context.DeliveryMethods
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
