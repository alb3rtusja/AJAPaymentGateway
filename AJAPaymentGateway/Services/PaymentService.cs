using AJAPaymentGateway.Core.Payments;
using AJAPaymentGateway.Data;
using AJAPaymentGateway.Models;

namespace AJAPaymentGateway.Services
{
    public class PaymentService
    {
        private readonly AppDbContext _db;
        private readonly WebhookService _webhookService;
        private readonly PaymentProcessorFactory _processorFactory;

        public PaymentService(AppDbContext db
            , WebhookService webhookService
            , PaymentProcessorFactory processorFactory)
        {
            _db = db;
            _webhookService = webhookService;
            _processorFactory = processorFactory;
        }

        public Payment CreatePayment(
            string orderId,
            int amount,
            string customerName,
            string callbackUrl)
        {
            var payment = new Payment
            {
                PaymentId = "pay_" + Guid.NewGuid().ToString("N")[..10],
                OrderId = orderId,
                Amount = amount,
                CustomerName = customerName,
                CallbackUrl = callbackUrl,
                Status = "pending",
                CreatedAt = DateTime.UtcNow
            };

            _db.Payments.Add(payment);
            _db.SaveChanges();

            return payment;
        }

        public Payment GetById(string id)
        {
            return _db.Payments.FirstOrDefault(p => p.PaymentId == id);
        }

        public async Task<bool> ProcessPaymentAsync(string paymentId)
        {
            var payment = GetById(paymentId);
            if (payment == null) return false;

            var processor = _processorFactory.Create();
            await processor.ProcessAsync(payment);

            _db.SaveChanges();

            if (payment.Status != "pending")
            {
                await _webhookService.SendAsync(payment);
            }

            return true;
        }






    }
}
