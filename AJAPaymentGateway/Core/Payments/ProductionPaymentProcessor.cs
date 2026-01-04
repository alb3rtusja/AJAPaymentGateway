using AJAPaymentGateway.Models;

namespace AJAPaymentGateway.Core.Payments
{
    public class ProductionPaymentProcessor : IPaymentProcessor
    {
        public Task ProcessAsync(Payment payment)
        {
            // NANTI: real bank / VA / card logic
            payment.Status = "paid";
            payment.UpdatedAt = DateTime.UtcNow;

            return Task.CompletedTask;
        }
    }
}
