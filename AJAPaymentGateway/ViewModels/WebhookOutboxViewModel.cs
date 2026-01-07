namespace AJAPaymentGateway.ViewModels
{
    public class WebhookOutboxViewModel
    {
        public Guid Id { get; set; }
        public string PaymentId { get; set; } = default!;
        public string TargetUrl { get; set; } = default!;
        public int RetryCount { get; set; }
        public bool IsSent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastAttemptAt { get; set; }
    }
}
