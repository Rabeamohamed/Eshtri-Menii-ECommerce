using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Core.Entities
{
    public class CustomerBasket
    {
        public CustomerBasket()
        {
            
        }
        public CustomerBasket(int id)
        {
            Id = id;
        }
        public int Id { get; set; } // key
        public List<BasketItem> BasketItems { get; set; } = new List<BasketItem>();  // Value
    }
}
