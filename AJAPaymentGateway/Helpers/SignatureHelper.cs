using System.Security.Cryptography;
using System.Text;

namespace AJAPaymentGateway.Helpers
{
    public static class SignatureHelper
    {
        public static string Generate(string payload, string secretKey)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            return Convert.ToHexString(hash).ToLower();
        }
    }
}
