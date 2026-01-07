using System.ComponentModel.DataAnnotations;

namespace AJAPaymentGateway.Models
{
    public class WebhookOutbox
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string PaymentId { get; set; } = default!;

        [Required]
        public string TargetUrl { get; set; } = default!;

        [Required]
        public string Payload { get; set; } = default!;

        public int RetryCount { get; set; } = 0;

        public DateTime? LastAttemptAt { get; set; }

        public DateTime NextRetryAt { get; set; } = DateTime.UtcNow;

        public bool IsSent { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
