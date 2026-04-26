using ECom.Core.Entities.Product;
using ECom.Core.Interfaces;
using ECom.Infrastructure.Data;

namespace ECom.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }
    }
}
