using AJAPaymentGateway.Models;

namespace AJAPaymentGateway.Core.Payments
{
    public interface IPaymentProcessor
    {
        Task ProcessAsync(Payment payment);
    }
}
