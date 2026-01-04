using System.ComponentModel.DataAnnotations;

namespace AJAPaymentGateway.DTOs
{
    /// <summary>
    /// Request to create a new payment
    /// </summary>
    public class CreatePaymentRequest
    {
        /// <summary>Merchant order ID. <b>Required</b></summary>
        /// <example>ORDER-1001</example>
        [Required]
        public string OrderId { get; set; } = default!;

        /// <summary>Total payment amount in IDR. <b>Required</b></summary>
        /// <example>50000</example>
        [Required]
        public int Amount { get; set; }

        /// <summary>Customer name</summary>
        /// <example>Alfred</example>
        public string CustomerName { get; set; } = default!;

        /// <summary>Webhook callback URL. <b>Required</b></summary>
        /// <example>https://merchant.test/webhook</example>
        [Required]
        [Url]
        public string CallbackUrl { get; set; } = default!;
    }
}
