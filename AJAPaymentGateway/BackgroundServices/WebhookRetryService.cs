using AJAPaymentGateway.Data;
using AJAPaymentGateway.Services;
using Microsoft.EntityFrameworkCore;

namespace AJAPaymentGateway.BackgroundServices
{
    public class WebhookRetryService : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private const int MAX_RETRY = 3;

        public WebhookRetryService(IServiceProvider provider)
        {
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await RetryFailedWebhooks(stoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task RetryFailedWebhooks(CancellationToken ct)
        {
            using var scope = _provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var webhookService = scope.ServiceProvider.GetRequiredService<IWebhookService>();

            var failedWebhooks = await db.WebhookLogs
                .Where(x =>
                    !x.IsSuccess &&
                    x.RetryCount < MAX_RETRY)
                .OrderBy(x => x.CreatedAt)
                .Take(10)
                .ToListAsync(ct);

            foreach (var log in failedWebhooks)
            {
                try
                {
                    await webhookService.RetryAsync(log);
                }
                catch
                {
                    // swallow error, will retry again later
                }
            }

            await db.SaveChangesAsync(ct);
        }
    }
}
