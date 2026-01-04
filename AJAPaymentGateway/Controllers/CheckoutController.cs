using AJAPaymentGateway.DTOs;
using AJAPaymentGateway.Models;
using AJAPaymentGateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace AJAPaymentGateway.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("Checkout")]
    public class CheckoutController : Controller
    {
        private readonly PaymentService _paymentService;

        public CheckoutController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("{id}")]
        public IActionResult Index(string id)
        {
            var payment = _paymentService.GetById(id);
            if (payment == null)
                return NotFound();

            return View(payment);
        }

        [HttpPost("{id}/pay")]
        public async Task <IActionResult> Pay(string id)
        {
            var success = await _paymentService.ProcessPaymentAsync(id);
            if (!success) return NotFound();

            return Redirect($"/checkout/{id}");
        }

        [HttpPost("{id}/fail")]
        public async Task <IActionResult> Fail(string id)
        {
            var success = await _paymentService.ProcessPaymentAsync(id);
            if (!success) return NotFound();

            return Redirect($"/checkout/{id}");
        }




    }
}
