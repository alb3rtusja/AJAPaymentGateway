using AJAPaymentGateway.Core.Payments;
using AJAPaymentGateway.Data;
using AJAPaymentGateway.DTOs;
using AJAPaymentGateway.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AJAPaymentGateway.Services
{
    public class PaymentService
    {
        private readonly AppDbContext _db;
        private readonly IWebhookService _webhookService;
        private readonly IPaymentProcessorFactory _processorFactory;

        public PaymentService(AppDbContext db
            , IWebhookService webhookService
            , IPaymentProcessorFactory processorFactory)
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
            var exists =  _db.Payments.Any(p => p.OrderId == orderId);

            if (exists)
                throw new InvalidOperationException(
                    $"Payment with OrderId {orderId} already exists");

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

            var outbox = new WebhookOutbox
            {
                PaymentId = payment.PaymentId,
                TargetUrl = payment.CallbackUrl,
                Payload = JsonSerializer.Serialize(payment)
            };

            _db.WebhookOutbox.Add(outbox);

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

        public async Task<Payment> CreateAsync(CreatePaymentRequest paymentRequest)
        {
            var exists = await _db.Payments
                .AnyAsync(p => p.OrderId == paymentRequest.OrderId);

            if (exists)
                throw new InvalidOperationException(
                    $"Payment with OrderId {paymentRequest.OrderId} already exists");

            var payment = new Payment
            {
                PaymentId = "pay_" + Guid.NewGuid().ToString("N")[..10],
                OrderId = paymentRequest.OrderId,
                Amount = paymentRequest.Amount,
                CustomerName = paymentRequest.CustomerName,
                CallbackUrl = paymentRequest.CallbackUrl,
                Status = "pending",
                CreatedAt = DateTime.UtcNow
            };

            _db.Payments.Add(payment);

            var outbox = new WebhookOutbox
            {
                PaymentId = payment.PaymentId,
                TargetUrl = payment.CallbackUrl,
                Payload = JsonSerializer.Serialize(payment)
            };

            _db.WebhookOutbox.Add(outbox);

            await _db.SaveChangesAsync();

            return payment;
        }





    }
}
