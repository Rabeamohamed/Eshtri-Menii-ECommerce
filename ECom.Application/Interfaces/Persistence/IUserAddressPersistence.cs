using ECom.Core.Entities;

namespace ECom.Application.Interfaces.Persistence
{
    public interface IUserAddressPersistence
    {
        Task<bool> UpsertUserAddressAsync(string userId, Address address, CancellationToken cancellationToken = default);
    }
}
