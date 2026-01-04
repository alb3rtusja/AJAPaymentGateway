namespace AJAPaymentGateway.DTOs
{
    public class CreatePaymentResponse
    {
        public string PaymentId { get; set; } = default!;
        public string CheckoutUrl { get; set; } = default!;
        public string Status { get; set; } = default!;
    }
}
