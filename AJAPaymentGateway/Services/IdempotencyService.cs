using AJAPaymentGateway.Data;
using AJAPaymentGateway.Models;
using System.Security.Cryptography;
using System.Text;

namespace AJAPaymentGateway.Services
{
    public class IdempotencyService
    {
        private readonly AppDbContext _db;

        public IdempotencyService(AppDbContext db)
        {
            _db = db;
        }

        public string HashRequest(string body)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(body));
            return Convert.ToHexString(bytes);
        }

        public IdempotencyRecord? Get(string key)
        {
            return _db.IdempotencyRecords.Find(key);
        }

        public void Save(
            string key,
            string requestHash,
            int statusCode,
            string responseBody)
        {
            _db.IdempotencyRecords.Add(new IdempotencyRecord
            {
                Key = key,
                RequestHash = requestHash,
                StatusCode = statusCode,
                ResponseBody = responseBody
            });

            _db.SaveChanges();
        }
    }
}
