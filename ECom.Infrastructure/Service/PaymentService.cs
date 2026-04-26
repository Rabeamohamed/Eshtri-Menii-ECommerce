using ECom.Core.Entities;
using ECom.Core.Entities.Order;
using ECom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using ECom.Application.Interfaces.Repositories;
using ECom.Application.Interfaces.Services;

namespace ECom.Infrastructure.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public PaymentService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            StripeConfiguration.ApiKey = _configuration["StripSettings:SecretKey"];
        }


        public async Task<CustomerBasket> CreateOrUpdatePaymentAsync(string basketId, int? deliveryMethodId)
        {
           var basket = await _unitOfWork.CustomerBasketRepository.GetBasketAsync(basketId);
           if (basket == null) return null;

           decimal shippingPrice = 0m;
           if(deliveryMethodId.HasValue)
           {
               var delivery = await _unitOfWork.DeliveryMethodRepository.GetByIdAsync(deliveryMethodId.Value);
               if (delivery != null)
               {
                   shippingPrice = delivery.Price;
               }
           }

           foreach (var item in basket.BasketItems)
           {
               var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.Id);
               if (product != null)
               {
                   item.Price = product.NewPrice;
               }
           }

           var paymentIntentService = new PaymentIntentService();
           PaymentIntent intent;

           if(string.IsNullOrEmpty(basket.PaymentIntentId))
           {
               var options = new PaymentIntentCreateOptions
               {
                   Amount = (long)(basket.BasketItems.Sum(x => x.Quantity * x.Price) * 100 + shippingPrice * 100),
                   Currency = "USD",
                   PaymentMethodTypes = new List<string> { "card" }
               };
               intent = await paymentIntentService.CreateAsync(options);
               basket.PaymentIntentId = intent.Id;
               basket.ClientSecret = intent.ClientSecret;
           }
           else
           {
               var options = new PaymentIntentUpdateOptions
               {
                   Amount = (long)(basket.BasketItems.Sum(x => x.Quantity * x.Price) * 100 + shippingPrice * 100),
               };
               await paymentIntentService.UpdateAsync(basket.PaymentIntentId, options);
            }

            await _unitOfWork.CustomerBasketRepository.UpdateBasketAsync(basket);
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
            var order = await _unitOfWork.OrderRepository
                .GetOrderByPaymentIntentIdAsync(paymentIntentId);

            if (order is null) return;

            order.Status = status;
            await _unitOfWork.OrderRepository.UpdateOrderAsync(order);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
