using AJAPaymentGateway.Data;
using Microsoft.AspNetCore.Mvc;

namespace AJAPaymentGateway.Controllers
{
    [Route("admin/webhook-logs")]
    public class AdminWebhookLogsController : Controller
    {
        private readonly AppDbContext _db;

        public AdminWebhookLogsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var logs = _db.WebhookLogs
                .OrderByDescending(w => w.CreatedAt)
                .Take(100)
                .ToList();

            return View(logs);
        }
    }
}
