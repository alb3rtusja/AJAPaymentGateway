using AJAPaymentGateway.Data;
using AJAPaymentGateway.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AJAPaymentGateway.Controllers
{
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            var model = new DashboardViewModel
            {
                TotalPayments = _db.Payments.Count(),
                PendingPayments = _db.Payments.Count(p => p.Status == "pending"),
                SuccessPayments = _db.Payments.Count(p => p.Status == "paid"),
                FailedPayments = _db.Payments.Count(p => p.Status == "failed")
            };

            return View(model);
        }

        [HttpGet("payments")]
        public IActionResult Payments()
        {
            var payments = _db.Payments
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return View(payments);
        }

        [HttpGet("PaymentDetail/{id}")]
        public IActionResult PaymentDetail(string id)
        {
            var payment = _db.Payments.FirstOrDefault(p => p.PaymentId == id);
            if (payment == null)
                return NotFound();

            return View(payment);
        }
    }
}
