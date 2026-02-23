using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pupitre.Notifications.Infrastructure.EF.Repositories;
using Pupitre.Notifications.Infrastructure.Redis.Options;
using Pupitre.Notifications.Infrastructure.Redis.Repositories;

namespace Pupitre.Notifications.Infrastructure.Redis.Services;

internal class NotificationRedisSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationRedisSyncService> _logger;
    private readonly RedisSyncOptions _options;

    public NotificationRedisSyncService(
        IServiceProvider serviceProvider,
        ILogger<NotificationRedisSyncService> logger,
        IOptions<RedisSyncOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled) return;
        while (!stoppingToken.IsCancellationRequested)
        {
            try { await SyncAsync(stoppingToken); }
            catch (Exception ex) { _logger.LogError(ex, "Error during Redis sync"); }
            await Task.Delay(_options.SyncIntervalMs, stoppingToken);
        }
    }

    private async Task SyncAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var postgresRepo = scope.ServiceProvider.GetRequiredService<NotificationPostgresRepository>();
        var redisRepo = scope.ServiceProvider.GetRequiredService<NotificationRedisRepository>();
        var items = await postgresRepo.BrowseAsync(cancellationToken);
        foreach (var item in items)
        {
            try { await redisRepo.AddAsync(item, cancellationToken); }
            catch (Exception ex) { _logger.LogWarning(ex, "Failed to sync to Redis"); }
        }
    }
}
