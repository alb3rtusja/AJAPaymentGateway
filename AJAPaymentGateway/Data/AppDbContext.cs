using AJAPaymentGateway.Models;
using Microsoft.EntityFrameworkCore;

namespace AJAPaymentGateway.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
        {
        }

        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<WebhookLog> WebhookLogs => Set<WebhookLog>();
        public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();



    }
}
