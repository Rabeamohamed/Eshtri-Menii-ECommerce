using ECom.Application.Interfaces.Repositories;
using ECom.Core.Entities;
using StackExchange.Redis;
using System.Text.Json;

namespace ECom.Infrastructure.Repositories
{
    public class CustomerBasketRepository : ICustomerBasketRepository
    {
        private readonly IDatabase _database;

        public CustomerBasketRepository(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public Task<bool> DeleteBasketAsync(string id)
            => _database.KeyDeleteAsync(id);

        public async Task<CustomerBasket?> GetBasketAsync(string id)
        {
            var result = await _database.StringGetAsync(id);
            if (result.IsNullOrEmpty)
                return null;

            return JsonSerializer.Deserialize<CustomerBasket>(result!);
        }

        public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket)
        {
            if (basket is null || string.IsNullOrWhiteSpace(basket.Id))
                return null;

            var ok = await _database.StringSetAsync(
                basket.Id,
                JsonSerializer.Serialize(basket),
                TimeSpan.FromDays(3));

            if (!ok)
                return null;

            return await GetBasketAsync(basket.Id);
        }
    }
}
