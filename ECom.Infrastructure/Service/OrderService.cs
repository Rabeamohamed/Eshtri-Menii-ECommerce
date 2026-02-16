using AutoMapper;
using ECom.Core.DTO.Order;
using ECom.Core.Entities.Order;
using ECom.Core.Interfaces;
using ECom.Core.Services;
using ECom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;


namespace ECom.Infrastructure.Service
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public OrderService(IUnitOfWork unitOfWork, AppDbContext context, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
        }

        public async Task<Orders> CreateOrderAsync(OrderDto orderDto, string BuyerEmail)
        {
            var basket = await _unitOfWork.CustomerBasketRepository.GetBasketAsync(orderDto.BasketId);

            List<OrderItems> orderItems = new List<OrderItems>();

            foreach(var item in basket.BasketItems)
            {
                var Product = await _unitOfWork.ProductRepository.GetByIdAsync(item.Id);
                var OrderItems = new OrderItems(Product.Id,item.Image,
                    Product.Name,item.Price,item.Quantity);
                orderItems.Add(OrderItems);
            }
            var deliveryMethod = await _context.DeliveryMethods.FirstOrDefaultAsync(d => d.Id == orderDto.DeliveryMethodId);    
            var subTotal = orderItems.Sum(o => o.Price * o.Quantity);

            var shippingAddress = _mapper.Map<ShippingAddress>(orderDto.ShippingAddress);
            var order = new Orders(BuyerEmail, subTotal, shippingAddress, deliveryMethod, orderItems); // Error Here

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            await _unitOfWork.CustomerBasketRepository.DeleteBasketAsync(orderDto.BasketId);
            return order;
        }

        public async Task<IReadOnlyList<Orders>> GetAllOrdersForUserAsync(string BuyerEmail)
        {
            var orders = await _context.Orders.Where( O=> O.BuyerEmail == BuyerEmail)
                .Include(OI => OI.OrderItems).Include(D => D.DeliveryMethod)
                .ToListAsync();
            return orders;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync()
        => await _context.DeliveryMethods.AsNoTracking().ToListAsync();

        public async Task<Orders> GetOrderByIdAsync(int id, string BuyerEmail)
        {
            var order = await _context.Orders.Where(O => O.Id == id && O.BuyerEmail == BuyerEmail)
                .Include(OI => OI.OrderItems).Include(D => D.DeliveryMethod)
                .FirstOrDefaultAsync();
            return order;
        }
    }
}
