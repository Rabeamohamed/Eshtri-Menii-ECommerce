using ECom.Application.Interfaces.Repositories;
using ECom.Core.Entities.Product;
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
