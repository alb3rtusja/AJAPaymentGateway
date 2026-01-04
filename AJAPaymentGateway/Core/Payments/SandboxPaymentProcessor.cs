using AJAPaymentGateway.Models;

namespace AJAPaymentGateway.Core.Payments
{
    public class SandboxPaymentProcessor : IPaymentProcessor
    {
        public Task ProcessAsync(Payment payment)
        {
            // RULE SIMULASI (realistic)
            if (payment.Amount <= 100_000)
            {
                payment.Status = "paid";
            }
            else if (payment.Amount <= 200_000)
            {
                payment.Status = "pending";
            }
            else
            {
                payment.Status = "failed";
            }

            payment.UpdatedAt = DateTime.UtcNow;

            return Task.CompletedTask;
        }
    }
}
