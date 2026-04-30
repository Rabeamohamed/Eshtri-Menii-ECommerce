namespace ECom.Application.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        public ICategoryRepository CategoryRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IPhotoRepository PhotoRepository { get;  }
        public ICustomerBasketRepository CustomerBasketRepository { get; }
        public IReviewRepository ReviewRepository { get; }
        public IWishlistRepository WishlistRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public IDeliveryMethodRepository DeliveryMethodRepository { get; } 
        public IAnalyticsRepository AnalyticsRepository { get; }
        public Task<int> SaveChangesAsync(); // int because return number of affected rows

    }
}
