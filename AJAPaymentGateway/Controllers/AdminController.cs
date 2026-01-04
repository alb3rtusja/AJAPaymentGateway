using AJAPaymentGateway.Data;
using AJAPaymentGateway.ViewModels;
using Microsoft.AspNetCore.Mvc;
using AJAPaymentGateway.Models;

namespace AJAPaymentGateway.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

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

        public IActionResult Payments()
        {
            var payments = _db.Payments
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return View(payments);
        }

        public IActionResult PaymentDetail(string id)
        {
            var payment = _db.Payments.FirstOrDefault(p => p.PaymentId == id);
            if (payment == null) return NotFound();

            return View(payment);
        }

        public IActionResult Webhooks()
        {
            var logs = _db.WebhookLogs
                .OrderByDescending(w => w.CreatedAt)
                .Take(100)
                .ToList();

            return View(logs);
        }




    }
}
