using AJAPaymentGateway.Data;
using AJAPaymentGateway.Models;
using System.Text.Json;
using System.Text;
using AJAPaymentGateway.Helpers;

namespace AJAPaymentGateway.Services
{
    public interface IWebhookService
    {
        Task SendAsync(Payment payment);
        Task RetryAsync(WebhookLog log);
        Task SendRawAsync(string url, string payload);
    }

    public class WebhookService : IWebhookService
    {
        private readonly HttpClient _http;
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public WebhookService(HttpClient http, AppDbContext db, IConfiguration config)
        {
            _http = http;
            _db = db;
            _config = config;
        }

        public async Task SendAsync(Payment payment)
        {
            var payloadObj = new
            {
                payment_id = payment.PaymentId,
                order_id = payment.OrderId,
                amount = payment.Amount,
                status = payment.Status,
                paid_at = DateTime.UtcNow
            };

            var payload = JsonSerializer.Serialize(payloadObj);

            var serverKey = _config["PaymentGateway:ServerKey"]!;
            var signature = SignatureHelper.Generate(payload, serverKey);

            var request = new HttpRequestMessage(HttpMethod.Post, payment.CallbackUrl);
            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
            request.Headers.Add("X-Signature", signature);

            int statusCode = 0;

            try
            {
                var response = await _http.SendAsync(request);
                statusCode = (int)response.StatusCode;
            }
            catch
            {
                statusCode = 0; // network error
            }

            var success = statusCode >= 200 && statusCode < 300;

            _db.WebhookLogs.Add(new WebhookLog
            {
                PaymentId = payment.PaymentId,
                Url = payment.CallbackUrl,
                Payload = payload,
                ResponseCode = statusCode,
                IsSuccess = success,
                RetryCount = 0,
                CreatedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
        }

        public async Task RetryAsync(WebhookLog log)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, log.Url);
            request.Content = new StringContent(log.Payload, Encoding.UTF8, "application/json");

            int statusCode;

            try
            {
                var response = await _http.SendAsync(request);
                statusCode = (int)response.StatusCode;
                log.IsSuccess = response.IsSuccessStatusCode;
            }
            catch
            {
                statusCode = 0;
                log.IsSuccess = false;
            }

            log.ResponseCode = statusCode;
            log.RetryCount++;
            log.LastRetryAt = DateTime.UtcNow;
        }

        public async Task SendRawAsync(string url, string payload)
        {
            var content = new StringContent(
                payload,
                Encoding.UTF8,
                "application/json");

            var response = await _http.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
        }




    }
}
