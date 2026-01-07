using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AJAPaymentGateway.Controllers
{
    [ApiController]
    [Route("test/webhook")]
    public class TestWebhookController : ControllerBase
    {
        [HttpPost]
        public IActionResult Receive([FromBody] object payload)
        {
            Console.WriteLine("WEBHOOK RECEIVED:");
            Console.WriteLine(JsonSerializer.Serialize(payload));
            return Ok();
        }
    }
}
