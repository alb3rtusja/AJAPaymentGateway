using System.ComponentModel.DataAnnotations;

namespace AJAPaymentGateway.DTOs
{
    public class CreatePaymentRequest
    {
        [Required]
        public string OrderId { get; set; } = default!;

        [Required]
        public int Amount { get; set; }

        public string CustomerName { get; set; } = default!;

        [Required]
        [Url]
        public string CallbackUrl { get; set; } = default!;
    }
}
