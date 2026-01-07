using AJAPaymentGateway.Data;
using AJAPaymentGateway.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AJAPaymentGateway.Controllers
{
    [Route("admin/webhook-outbox")]
    public class AdminWebhookController : Controller
    {
        private readonly AppDbContext _db;

        public AdminWebhookController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var data = _db.WebhookOutbox
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new WebhookOutboxViewModel
                {
                    Id = x.Id,
                    PaymentId = x.PaymentId,
                    TargetUrl = x.TargetUrl,
                    RetryCount = x.RetryCount,
                    IsSent = x.IsSent,
                    CreatedAt = x.CreatedAt,
                    LastAttemptAt = x.LastAttemptAt
                })
                .ToList();

            return View(data);
        }

        [HttpPost("retry/{id}")]
        public async Task<IActionResult> Retry(Guid id)
        {
            var job = await _db.WebhookOutbox.FindAsync(id);
            if (job == null)
                return NotFound();

            job.NextRetryAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            TempData["success"] = "Webhook scheduled for retry";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("retry-all")]
        public async Task<IActionResult> RetryAll()
        {
            var jobs = _db.WebhookOutbox
                .Where(x => !x.IsSent);

            foreach (var job in jobs)
                job.NextRetryAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            TempData["success"] = "All pending webhooks scheduled for retry";
            return RedirectToAction(nameof(Index));
        }
    }
}
