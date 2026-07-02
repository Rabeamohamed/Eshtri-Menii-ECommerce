using ECom.Core.Entities;

namespace ECom.Application.Interfaces.Repositories
{
    public interface ICouponRepository : IGenericRepository<Coupon>
    {
        Task<Coupon> GetCouponByCodeAsync(string code);
    }
}
