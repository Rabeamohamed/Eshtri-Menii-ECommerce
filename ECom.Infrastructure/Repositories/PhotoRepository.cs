using ECom.Core.Entities.Product;
using ECom.Core.Interfaces;
using ECom.Infrastructure.Data;


namespace ECom.Infrastructure.Repositories
{
    public class PhotoRepository : GenericRepository<Photo>, IPhotoRepository
    {
        public PhotoRepository(AppDbContext context) : base(context)
        {
        }
    }
}
