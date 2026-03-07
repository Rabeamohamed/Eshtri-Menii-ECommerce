using ECom.Core.Entities;
using ECom.Core.Entities.Order;
using ECom.Core.Services;
using ECom.Core.Sharing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace ECom.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _configuration;

        public PaymentController(IPaymentService paymentService,IConfiguration configuration)
        {
            _paymentService = paymentService;
            _configuration = configuration;
        }


        [HttpPost("Create")]
        public async Task<IActionResult> CreateOrUpdatePayment(string basketId, int? deliveryMethodId)
        {
            try
            {
                var basket = await _paymentService.CreateOrUpdatePaymentAsync(basketId, deliveryMethodId);
                if (basket is null)
                    return BadRequest(new ResponseAPI(400, "Problem creating payment"));
                return Ok(basket);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        // ✅ Webhook — NO [Authorize] — Stripe calls this, not the user!
        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook()
        {
            // 1. Read raw body — must be raw for signature verification
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                // 2. Verify signature — prevents fake requests
                var signature = HttpContext.Request.Headers["Stripe-Signature"];
                var whSecret = _configuration["StripSettings:WebSecret"];

                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    signature,
                    whSecret,
                    throwOnApiVersionMismatch: false
                );

                // 3. Handle events
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

                    default:
                        break;
                }

                // ✅ Always return 200 to Stripe — even if we don't handle the event
                return Ok();
            }
            catch (StripeException ex)
            {
                // Signature verification failed
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseAPI(500, ex.Message));
            }
        }
    }
}
