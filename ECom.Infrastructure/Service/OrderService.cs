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
            var order = new Orders(BuyerEmail, subTotal, shippingAddress, deliveryMethod, orderItems);

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public Task<IReadOnlyList<Orders>> GetAllOrdersForUserAsync(string BuyerEmail)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<DeliveryMethod>> GetDealMethodAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Orders> GetOrderByIdAsync(int id, string BuyerEmail)
        {
            throw new NotImplementedException();
        }
    }
}
