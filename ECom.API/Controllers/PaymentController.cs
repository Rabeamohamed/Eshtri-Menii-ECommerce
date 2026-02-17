using ECom.Core.Entities;
using ECom.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        [HttpPost("Create")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePayment(string basketId, int? deliveryMethodId)
        {
            return await paymentService.CreateOrUpdatePaymentAsync(basketId, deliveryMethodId);
        }
    }
}
