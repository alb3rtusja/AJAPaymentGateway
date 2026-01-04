using AJAPaymentGateway.DTOs;
using AJAPaymentGateway.Services;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AJAPaymentGateway.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly IConfiguration _config;
        private readonly IdempotencyService _idempotencyService;

        public PaymentsController(PaymentService paymentService
            , IConfiguration config
            , IdempotencyService idempotencyService)
        {
            _paymentService = paymentService;
            _config = config;
            _idempotencyService = idempotencyService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CreatePaymentResponse), 201)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult CreatePayment([FromBody] CreatePaymentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var idemKey = Request.Headers["Idempotency-Key"].FirstOrDefault();
            var bodyJson = JsonSerializer.Serialize(request);

            if (!string.IsNullOrEmpty(idemKey))
            {
                var hash = _idempotencyService.HashRequest(bodyJson);
                var existing = _idempotencyService.Get(idemKey);

                if (existing != null)
                {
                    if (existing.RequestHash != hash)
                        return Conflict("Idempotency-Key reuse with different payload");

                    return StatusCode(
                        existing.StatusCode,
                        JsonSerializer.Deserialize<object>(existing.ResponseBody));
                }
            }

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

            var responseJson = JsonSerializer.Serialize(response);

            if (!string.IsNullOrEmpty(idemKey))
            {
                _idempotencyService.Save(
                    idemKey,
                    _idempotencyService.HashRequest(bodyJson),
                    201,
                    responseJson);
            }

            return Ok(response);
        }




    }
}
