#!/bin/bash
# add-persistence-layers.sh - Adds Redis repositories, Composite repositories, and Sync services
# Usage: ./add-persistence-layers.sh <SERVICE_TYPE> <SERVICE_NAME> <ENTITY_NAME>
# Example: ./add-persistence-layers.sh Foundation Users User

SERVICE_TYPE=$1  # Foundation, AI, Support
SERVICE_NAME=$2  # Users, Lessons, etc.
ENTITY_NAME=$3   # User, Lesson, etc.

if [ -z "$SERVICE_TYPE" ] || [ -z "$SERVICE_NAME" ] || [ -z "$ENTITY_NAME" ]; then
    echo "Usage: $0 <SERVICE_TYPE> <SERVICE_NAME> <ENTITY_NAME>"
    exit 1
fi

cd /Volumes/Barracuda/mamey-io/code-final/Pupitre
BASE="src/Services/${SERVICE_TYPE}/Pupitre.${SERVICE_NAME}/src/Pupitre.${SERVICE_NAME}.Infrastructure"

# Create directories
mkdir -p "$BASE/Redis/Repositories" "$BASE/Redis/Services" "$BASE/Redis/Options"
mkdir -p "$BASE/Composite"
mkdir -p "$BASE/Mongo/Services" "$BASE/Mongo/Options"

# Create Redis Repository
cat > "$BASE/Redis/Repositories/${ENTITY_NAME}RedisRepository.cs" << EOF
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Pupitre.${SERVICE_NAME}.Domain.Entities;
using Pupitre.${SERVICE_NAME}.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.${SERVICE_NAME}.Infrastructure.Redis.Repositories;

internal class ${ENTITY_NAME}RedisRepository : I${ENTITY_NAME}Repository
{
    private readonly IDatabase _database;
    private readonly ILogger<${ENTITY_NAME}RedisRepository> _logger;
    private const string KeyPrefix = "${ENTITY_NAME,,}:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public ${ENTITY_NAME}RedisRepository(IConnectionMultiplexer redis, ILogger<${ENTITY_NAME}RedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(${ENTITY_NAME} entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<${ENTITY_NAME}?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null;
        return JsonSerializer.Deserialize<${ENTITY_NAME}>(json!);
    }

    public async Task UpdateAsync(${ENTITY_NAME} entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<${ENTITY_NAME}>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<${ENTITY_NAME}>>(Array.Empty<${ENTITY_NAME}>());

    private static string GetKey(Guid id) => \$"{KeyPrefix}{id}";
}
EOF

# Create Redis Extensions
cat > "$BASE/Redis/Extensions.cs" << EOF
using Microsoft.Extensions.DependencyInjection;
using Mamey.Microservice.Infrastructure;
using Pupitre.${SERVICE_NAME}.Infrastructure.Redis.Repositories;
using Pupitre.${SERVICE_NAME}.Infrastructure.Redis.Services;

namespace Pupitre.${SERVICE_NAME}.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<${ENTITY_NAME}RedisRepository>();
        builder.Services.AddHostedService<${ENTITY_NAME}RedisSyncService>();
        return builder;
    }
}
EOF

# Create Redis Sync Service
cat > "$BASE/Redis/Services/${ENTITY_NAME}RedisSyncService.cs" << EOF
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pupitre.${SERVICE_NAME}.Infrastructure.EF.Repositories;
using Pupitre.${SERVICE_NAME}.Infrastructure.Redis.Options;
using Pupitre.${SERVICE_NAME}.Infrastructure.Redis.Repositories;

namespace Pupitre.${SERVICE_NAME}.Infrastructure.Redis.Services;

internal class ${ENTITY_NAME}RedisSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<${ENTITY_NAME}RedisSyncService> _logger;
    private readonly RedisSyncOptions _options;

    public ${ENTITY_NAME}RedisSyncService(
        IServiceProvider serviceProvider,
        ILogger<${ENTITY_NAME}RedisSyncService> logger,
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
        var postgresRepo = scope.ServiceProvider.GetRequiredService<${ENTITY_NAME}PostgresRepository>();
        var redisRepo = scope.ServiceProvider.GetRequiredService<${ENTITY_NAME}RedisRepository>();
        var items = await postgresRepo.BrowseAsync(cancellationToken);
        foreach (var item in items)
        {
            try { await redisRepo.AddAsync(item, cancellationToken); }
            catch (Exception ex) { _logger.LogWarning(ex, "Failed to sync to Redis"); }
        }
    }
}
EOF

# Create Redis Options
cat > "$BASE/Redis/Options/RedisSyncOptions.cs" << EOF
namespace Pupitre.${SERVICE_NAME}.Infrastructure.Redis.Options;

public class RedisSyncOptions
{
    public const string SectionName = "Redis:Sync";
    public bool Enabled { get; set; } = true;
    public int SyncIntervalMs { get; set; } = 60000;
    public int BatchSize { get; set; } = 100;
}
EOF

# Create Composite Repository
cat > "$BASE/Composite/Composite${ENTITY_NAME}Repository.cs" << EOF
using Microsoft.Extensions.Logging;
using Pupitre.${SERVICE_NAME}.Domain.Entities;
using Pupitre.${SERVICE_NAME}.Domain.Repositories;
using Pupitre.${SERVICE_NAME}.Infrastructure.EF.Repositories;
using Pupitre.${SERVICE_NAME}.Infrastructure.Mongo.Repositories;
using Pupitre.${SERVICE_NAME}.Infrastructure.Redis.Repositories;

namespace Pupitre.${SERVICE_NAME}.Infrastructure.Composite;

internal class Composite${ENTITY_NAME}Repository : I${ENTITY_NAME}Repository
{
    private readonly ${ENTITY_NAME}PostgresRepository _postgresRepo;
    private readonly ${ENTITY_NAME}MongoRepository _mongoRepo;
    private readonly ${ENTITY_NAME}RedisRepository _redisRepo;
    private readonly ILogger<Composite${ENTITY_NAME}Repository> _logger;

    public Composite${ENTITY_NAME}Repository(
        ${ENTITY_NAME}PostgresRepository postgresRepo,
        ${ENTITY_NAME}MongoRepository mongoRepo,
        ${ENTITY_NAME}RedisRepository redisRepo,
        ILogger<Composite${ENTITY_NAME}Repository> logger)
    {
        _postgresRepo = postgresRepo;
        _mongoRepo = mongoRepo;
        _redisRepo = redisRepo;
        _logger = logger;
    }

    public async Task AddAsync(${ENTITY_NAME} entity, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.AddAsync(entity, cancellationToken);
        await TryAsync(() => _redisRepo.AddAsync(entity, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.AddAsync(entity, cancellationToken), "MongoDB");
    }

    public async Task<${ENTITY_NAME}?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try { var r = await _redisRepo.GetAsync(id, cancellationToken); if (r != null) return r; }
        catch { _logger.LogWarning("Redis read failed, trying MongoDB"); }
        try { var m = await _mongoRepo.GetAsync(id, cancellationToken); if (m != null) return m; }
        catch { _logger.LogWarning("MongoDB read failed, trying PostgreSQL"); }
        return await _postgresRepo.GetAsync(id, cancellationToken);
    }

    public async Task UpdateAsync(${ENTITY_NAME} entity, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.UpdateAsync(entity, cancellationToken);
        await TryAsync(() => _redisRepo.UpdateAsync(entity, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.UpdateAsync(entity, cancellationToken), "MongoDB");
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.DeleteAsync(id, cancellationToken);
        await TryAsync(() => _redisRepo.DeleteAsync(id, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.DeleteAsync(id, cancellationToken), "MongoDB");
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try { if (await _redisRepo.ExistsAsync(id, cancellationToken)) return true; } catch { }
        try { if (await _mongoRepo.ExistsAsync(id, cancellationToken)) return true; } catch { }
        return await _postgresRepo.ExistsAsync(id, cancellationToken);
    }

    public Task<IReadOnlyList<${ENTITY_NAME}>> BrowseAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.BrowseAsync(cancellationToken);

    private async Task TryAsync(Func<Task> action, string store)
    {
        try { await action(); }
        catch (Exception ex) { _logger.LogWarning(ex, "Propagation to {Store} failed", store); }
    }
}
EOF

# Create Composite Extensions
cat > "$BASE/Composite/Extensions.cs" << EOF
using Microsoft.Extensions.DependencyInjection;
using Mamey.Microservice.Infrastructure;
using Pupitre.${SERVICE_NAME}.Domain.Repositories;

namespace Pupitre.${SERVICE_NAME}.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<I${ENTITY_NAME}Repository, Composite${ENTITY_NAME}Repository>();
        return builder;
    }
}
EOF

echo "Created persistence layers for Pupitre.${SERVICE_NAME}"
