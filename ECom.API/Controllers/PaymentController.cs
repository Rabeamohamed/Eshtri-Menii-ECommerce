using ECom.Application.Interfaces.Services;
using ECom.Application.Sharing;
using ECom.Core.Entities.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace ECom.API.Controllers
{
    //Customer payment intent (auth). Webhook is anonymous for Stripe.</summary>
    [Authorize]
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _configuration;

        public PaymentController(IPaymentService paymentService, IConfiguration configuration)
        {
            _paymentService = paymentService;
            _configuration = configuration;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateOrUpdatePayment([FromQuery] string basketId, [FromQuery] int? deliveryMethodId)
        {
            if (string.IsNullOrWhiteSpace(basketId))
                return BadRequest(new ResponseAPI(400, "Basket id is required."));

            var basket = await _paymentService.CreateOrUpdatePaymentAsync(basketId, deliveryMethodId);
            if (basket is null)
                return BadRequest(new ResponseAPI(400, "Problem creating payment"));

            return Ok(basket);
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var signature = HttpContext.Request.Headers["Stripe-Signature"].ToString();
            var whSecret = _configuration["StripSettings:WebSecret"];
            if (string.IsNullOrWhiteSpace(whSecret))
                return StatusCode(500, new ResponseAPI(500, "Webhook secret is not configured."));

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    signature,
                    whSecret,
                    throwOnApiVersionMismatch: false
                );

                switch (stripeEvent.Type)
                {
                    case "payment_intent.succeeded":
                        var succeededIntent = stripeEvent.Data.Object as PaymentIntent;
                        if (succeededIntent is not null)
                            await _paymentService.UpdateOrderPaymentStatusAsync(
                                succeededIntent.Id,
                                PaymentStatus.PaymentReceived);
                        break;

                    case "payment_intent.payment_failed":
                        var failedIntent = stripeEvent.Data.Object as PaymentIntent;
                        if (failedIntent is not null)
                            await _paymentService.UpdateOrderPaymentStatusAsync(
                                failedIntent.Id,
                                PaymentStatus.PaymentFailed);
                        break;
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }
    }
}
