using ECom.Application.Interfaces.Persistence;
using ECom.Core.Entities;
using ECom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECom.Infrastructure.Persistence
{
    public class UserAddressPersistence : IUserAddressPersistence
    {
        private readonly AppDbContext _context;

        public UserAddressPersistence(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UpsertUserAddressAsync(string userId, Address address, CancellationToken cancellationToken = default)
        {
            var existing = await _context.Addresses
                .FirstOrDefaultAsync(a => a.AppUserId == userId, cancellationToken);

            if (existing is null)
            {
                address.AppUserId = userId;
                await _context.Addresses.AddAsync(address, cancellationToken);
            }
            else
            {
                address.Id = existing.Id;
                address.AppUserId = userId;
                _context.Entry(existing).State = EntityState.Detached;
                _context.Addresses.Update(address);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
