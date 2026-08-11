using Microsoft.EntityFrameworkCore;
using MyWeb.Portal.Data;

namespace MyWeb.Portal.Apps;

public sealed class AppHealthCheckWorker(
    IServiceScopeFactory scopeFactory,
    IHttpClientFactory httpClientFactory,
    ILogger<AppHealthCheckWorker> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(30);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        using var timer = new PeriodicTimer(Interval);

        do
        {
            await CheckAllAsync(stoppingToken);
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task CheckAllAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var applications = await db.AppDefinitions
            .Where(app => app.Enabled)
            .ToListAsync(cancellationToken);
        var client = httpClientFactory.CreateClient("AppHealth");

        foreach (var app in applications)
        {
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(TimeSpan.FromSeconds(5));

            try
            {
                var healthUri = new Uri(new Uri(app.InternalUrl), app.HealthPath.TrimStart('/'));
                using var response = await client.GetAsync(
                    healthUri,
                    HttpCompletionOption.ResponseHeadersRead,
                    timeout.Token);
                app.LastHealthy = response.IsSuccessStatusCode;
                app.LastHealthError = response.IsSuccessStatusCode
                    ? null
                    : $"HTTP {(int)response.StatusCode}";
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                app.LastHealthy = false;
                app.LastHealthError = "Timeout";
            }
            catch (HttpRequestException exception)
            {
                app.LastHealthy = false;
                app.LastHealthError = exception.Message.Length > 500
                    ? exception.Message[..500]
                    : exception.Message;
            }

            app.LastCheckedAt = DateTimeOffset.UtcNow;
        }

        if (applications.Count > 0)
        {
            await db.SaveChangesAsync(cancellationToken);
            logger.LogDebug("Updated health state for {Count} applications.", applications.Count);
        }
    }
}
