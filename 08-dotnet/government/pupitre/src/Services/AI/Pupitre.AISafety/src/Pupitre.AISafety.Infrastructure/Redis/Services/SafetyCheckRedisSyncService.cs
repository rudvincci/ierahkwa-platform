using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pupitre.AISafety.Infrastructure.EF.Repositories;
using Pupitre.AISafety.Infrastructure.Redis.Options;
using Pupitre.AISafety.Infrastructure.Redis.Repositories;

namespace Pupitre.AISafety.Infrastructure.Redis.Services;

internal class SafetyCheckRedisSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SafetyCheckRedisSyncService> _logger;
    private readonly RedisSyncOptions _options;

    public SafetyCheckRedisSyncService(
        IServiceProvider serviceProvider,
        ILogger<SafetyCheckRedisSyncService> logger,
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
        var postgresRepo = scope.ServiceProvider.GetRequiredService<SafetyCheckPostgresRepository>();
        var redisRepo = scope.ServiceProvider.GetRequiredService<SafetyCheckRedisRepository>();
        var items = await postgresRepo.BrowseAsync(cancellationToken);
        foreach (var item in items)
        {
            try { await redisRepo.AddAsync(item, cancellationToken); }
            catch (Exception ex) { _logger.LogWarning(ex, "Failed to sync to Redis"); }
        }
    }
}
