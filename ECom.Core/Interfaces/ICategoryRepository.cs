using ECom.Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Core.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        // Interface Inherit Interface for add a new and special methods for Category only 
    }
}
