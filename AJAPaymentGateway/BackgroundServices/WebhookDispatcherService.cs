using AJAPaymentGateway.Data;
using AJAPaymentGateway.Services;
using Microsoft.EntityFrameworkCore;

namespace AJAPaymentGateway.BackgroundServices
{
    public class WebhookDispatcherService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private const int MAX_RETRY = 5;


        public WebhookDispatcherService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var webhook = scope.ServiceProvider.GetRequiredService<IWebhookService>();

                var jobs = await db.WebhookOutbox
                    .Where(x =>
                        !x.IsSent &&
                        x.RetryCount < MAX_RETRY &&
                        x.NextRetryAt <= DateTime.UtcNow)
                    .Take(10)
                    .ToListAsync(stoppingToken);

                foreach (var job in jobs)
                {
                    try
                    {
                        await webhook.SendRawAsync(job.TargetUrl, job.Payload);

                        job.IsSent = true;
                    }
                    catch
                    {
                        job.RetryCount++;
                        job.LastAttemptAt = DateTime.UtcNow;
                        job.NextRetryAt = DateTime.UtcNow.AddMinutes(job.RetryCount * 2);
                    }
                }

                await db.SaveChangesAsync(stoppingToken);
                await Task.Delay(3000, stoppingToken);
            }
        }







    }
}
