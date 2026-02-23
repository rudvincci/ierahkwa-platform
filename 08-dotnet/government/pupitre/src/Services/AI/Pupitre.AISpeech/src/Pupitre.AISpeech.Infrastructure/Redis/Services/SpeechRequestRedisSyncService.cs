using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pupitre.AISpeech.Infrastructure.EF.Repositories;
using Pupitre.AISpeech.Infrastructure.Redis.Options;
using Pupitre.AISpeech.Infrastructure.Redis.Repositories;

namespace Pupitre.AISpeech.Infrastructure.Redis.Services;

internal class SpeechRequestRedisSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SpeechRequestRedisSyncService> _logger;
    private readonly RedisSyncOptions _options;

    public SpeechRequestRedisSyncService(
        IServiceProvider serviceProvider,
        ILogger<SpeechRequestRedisSyncService> logger,
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
        var postgresRepo = scope.ServiceProvider.GetRequiredService<SpeechRequestPostgresRepository>();
        var redisRepo = scope.ServiceProvider.GetRequiredService<SpeechRequestRedisRepository>();
        var items = await postgresRepo.BrowseAsync(cancellationToken);
        foreach (var item in items)
        {
            try { await redisRepo.AddAsync(item, cancellationToken); }
            catch (Exception ex) { _logger.LogWarning(ex, "Failed to sync to Redis"); }
        }
    }
}
