using AJAPaymentGateway.DTOs;
using AJAPaymentGateway.Services;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;

namespace AJAPaymentGateway.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly IConfiguration _config;

        public PaymentsController(PaymentService paymentService, IConfiguration config)
        {
            _paymentService = paymentService;
            _config = config;
        }

        [HttpPost]
        public IActionResult CreatePayment([FromBody] CreatePaymentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var payment = _paymentService.CreatePayment(
                request.OrderId,
                request.Amount,
                request.CustomerName,
                request.CallbackUrl
            );

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var response = new CreatePaymentResponse
            {
                PaymentId = payment.PaymentId,
                CheckoutUrl = $"{baseUrl}/checkout/{payment.PaymentId}",
                Status = payment.Status
            };

            return Ok(response);
        }




    }
}
