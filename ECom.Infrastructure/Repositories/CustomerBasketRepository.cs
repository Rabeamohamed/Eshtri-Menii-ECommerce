using ECom.Core.Entities;
using ECom.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Infrastructure.Repositories
{
    public class CustomerBasketRepository : ICustomerBasketRepository
    {
        public Task<bool> DeleteBasketAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<CustomerBasket> GetBasketAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            throw new NotImplementedException();
        }
    }
}
