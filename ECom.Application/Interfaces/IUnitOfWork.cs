using ECom.Core.Interfaces;
using ECom.Application.Interfaces;

namespace ECom.Application.Interfaces
{
    public interface IUnitOfWork
    {
        public ICategoryRepository CategoryRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IPhotoRepository PhotoRepository { get;  }
        public ICustomerBasketRepository CustomerBasketRepository { get; }
        public IAuth AuthRepository { get; }
        public IReviewRepository ReviewRepository { get; }
        public IWishlistRepository WishlistRepository { get; }
        public Task<int> SaveChangesAsync(); // int because return number of affected rows

    }
}
