using ECom.Core.Entities;
using ECom.Core.Entities.Order;
using ECom.Core.Interfaces;
using ECom.Core.Services;
using ECom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace ECom.Infrastructure.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _work;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public PaymentService(IUnitOfWork work, IConfiguration configuration, AppDbContext context)
        {
            _work = work;
            _configuration = configuration;
            _context = context;
            StripeConfiguration.ApiKey = _configuration["StripSettings:SecretKey"];
        }


        public async Task<CustomerBasket> CreateOrUpdatePaymentAsync(string basketId, int? deliveryMethodId)
        {
           var basket = await _work.CustomerBasketRepository.GetBasketAsync(basketId);

           decimal shippingPrice = 0m;
           if(deliveryMethodId.HasValue)
           {
               var delivery = await _context.DeliveryMethods.AsNoTracking()
                   .FirstOrDefaultAsync(x => x.Id == deliveryMethodId.Value);
               shippingPrice = delivery.Price;
           }

           foreach (var item in basket.BasketItems)
           {
               var product = await _work.ProductRepository.GetByIdAsync(item.Id);
               item.Price= product.NewPrice;
           }
           PaymentIntentService paymentIntentService = new PaymentIntentService();
           PaymentIntent _intent;

           if(string.IsNullOrEmpty(basket.PaymentIntentId))
           {
               var options = new PaymentIntentCreateOptions
               {
                   Amount = (long)basket.BasketItems.Sum(x => x.Quantity * (x.Price * 100) )+ (long)(shippingPrice * 100),
                   Currency = "USD",
                   PaymentMethodTypes = new List<string> { "card" }
               };
               _intent = await paymentIntentService.CreateAsync(options);
               basket.PaymentIntentId= _intent.Id;
               basket.ClientSecret= _intent.ClientSecret;
           }
           else
           {
               var options = new PaymentIntentUpdateOptions
               {
                   Amount = (long)basket.BasketItems.Sum(x => x.Quantity * (x.Price * 100)) + (long)(shippingPrice * 100),
               };
               await paymentIntentService.UpdateAsync(basket.PaymentIntentId, options);
            }
              await _work.CustomerBasketRepository.UpdateBasketAsync(basket);
                return basket;
        }

        public async Task<bool> RefundPaymentAsync(string paymentIntentId)
        {
            var refundOptions = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId,
                Reason = RefundReasons.RequestedByCustomer
            };

            var refundService = new RefundService();
            var refund = await refundService.CreateAsync(refundOptions);

            return refund.Status == "succeeded" || refund.Status == "pending";
        }

        public async Task UpdateOrderPaymentStatusAsync(string paymentIntentId, PaymentStatus status)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.PaymentIntentId == paymentIntentId);

            if (order is null) return;

            order.Status = status;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
