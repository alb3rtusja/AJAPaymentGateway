using System.ComponentModel.DataAnnotations;

namespace AJAPaymentGateway.Models
{
    public class WebhookLog
    {
        [Key]
        public Guid Id { get; set; }

        public string PaymentId { get; set; } = default!;
        public string Url { get; set; } = default!;
        public string Payload { get; set; } = default!;
        public int ResponseCode { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
