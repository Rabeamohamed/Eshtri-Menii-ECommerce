using ECom.Application.Interfaces.Repositories;
using ECom.Core.Entities;
using ECom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECom.Infrastructure.Repositories
{
    public class CouponRepository : GenericRepository<Coupon>, ICouponRepository
    {
        private readonly AppDbContext _context;
        public CouponRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Coupon> GetCouponByCodeAsync(string code)
        {
            return await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code.ToUpper() == code.ToUpper());
        }
    }
}
