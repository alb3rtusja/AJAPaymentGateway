using System.ComponentModel.DataAnnotations;

namespace AJAPaymentGateway.Models
{
    public class Payment
    {
        [Key]
        public string PaymentId { get; set; } = default!;

        [Required]
        public string OrderId { get; set; } = default!;

        [Required]
        public int Amount { get; set; }

        public string CustomerName { get; set; } = default!;

        [Required]
        public string CallbackUrl { get; set; } = default!;

        [Required]
        public string Status { get; set; } = "pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
