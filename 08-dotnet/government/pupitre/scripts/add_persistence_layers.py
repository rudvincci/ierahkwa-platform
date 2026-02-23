#!/usr/bin/env python3
"""
Add Redis repositories, Composite repositories, and Sync services to Pupitre microservices.
"""

import os
import sys

def create_redis_repository(base_path, service_name, entity_name):
    content = f'''using System.Text.Json;
using Microsoft.Extensions.Logging;
using Pupitre.{service_name}.Domain.Entities;
using Pupitre.{service_name}.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.{service_name}.Infrastructure.Redis.Repositories;

internal class {entity_name}RedisRepository : I{entity_name}Repository
{{
    private readonly IDatabase _database;
    private readonly ILogger<{entity_name}RedisRepository> _logger;
    private const string KeyPrefix = "{entity_name.lower()}:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public {entity_name}RedisRepository(IConnectionMultiplexer redis, ILogger<{entity_name}RedisRepository> logger)
    {{
        _database = redis.GetDatabase();
        _logger = logger;
    }}

    public async Task AddAsync({entity_name} entity, CancellationToken cancellationToken = default)
    {{
        var key = GetKey(entity.Id);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }}

    public async Task<{entity_name}?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {{
        var key = GetKey(id);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null;
        return JsonSerializer.Deserialize<{entity_name}>(json!);
    }}

    public async Task UpdateAsync({entity_name} entity, CancellationToken cancellationToken = default)
    {{
        var key = GetKey(entity.Id);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }}

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {{
        var key = GetKey(id);
        await _database.KeyDeleteAsync(key);
    }}

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {{
        var key = GetKey(id);
        return await _database.KeyExistsAsync(key);
    }}

    public Task<IReadOnlyList<{entity_name}>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<{entity_name}>>(Array.Empty<{entity_name}>());

    private static string GetKey(Guid id) => $"{{KeyPrefix}}{{id}}";
}}
'''
    path = os.path.join(base_path, "Redis", "Repositories", f"{entity_name}RedisRepository.cs")
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with open(path, 'w') as f:
        f.write(content)

def create_redis_extensions(base_path, service_name, entity_name):
    content = f'''using Microsoft.Extensions.DependencyInjection;
using Mamey.Microservice.Infrastructure;
using Pupitre.{service_name}.Infrastructure.Redis.Repositories;
using Pupitre.{service_name}.Infrastructure.Redis.Services;

namespace Pupitre.{service_name}.Infrastructure.Redis;

internal static class Extensions
{{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {{
        builder.Services.AddScoped<{entity_name}RedisRepository>();
        builder.Services.AddHostedService<{entity_name}RedisSyncService>();
        return builder;
    }}
}}
'''
    path = os.path.join(base_path, "Redis", "Extensions.cs")
    with open(path, 'w') as f:
        f.write(content)

def create_redis_sync_service(base_path, service_name, entity_name):
    content = f'''using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pupitre.{service_name}.Infrastructure.EF.Repositories;
using Pupitre.{service_name}.Infrastructure.Redis.Options;
using Pupitre.{service_name}.Infrastructure.Redis.Repositories;

namespace Pupitre.{service_name}.Infrastructure.Redis.Services;

internal class {entity_name}RedisSyncService : BackgroundService
{{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<{entity_name}RedisSyncService> _logger;
    private readonly RedisSyncOptions _options;

    public {entity_name}RedisSyncService(
        IServiceProvider serviceProvider,
        ILogger<{entity_name}RedisSyncService> logger,
        IOptions<RedisSyncOptions> options)
    {{
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }}

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {{
        if (!_options.Enabled) return;
        while (!stoppingToken.IsCancellationRequested)
        {{
            try {{ await SyncAsync(stoppingToken); }}
            catch (Exception ex) {{ _logger.LogError(ex, "Error during Redis sync"); }}
            await Task.Delay(_options.SyncIntervalMs, stoppingToken);
        }}
    }}

    private async Task SyncAsync(CancellationToken cancellationToken)
    {{
        using var scope = _serviceProvider.CreateScope();
        var postgresRepo = scope.ServiceProvider.GetRequiredService<{entity_name}PostgresRepository>();
        var redisRepo = scope.ServiceProvider.GetRequiredService<{entity_name}RedisRepository>();
        var items = await postgresRepo.BrowseAsync(cancellationToken);
        foreach (var item in items)
        {{
            try {{ await redisRepo.AddAsync(item, cancellationToken); }}
            catch (Exception ex) {{ _logger.LogWarning(ex, "Failed to sync to Redis"); }}
        }}
    }}
}}
'''
    path = os.path.join(base_path, "Redis", "Services", f"{entity_name}RedisSyncService.cs")
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with open(path, 'w') as f:
        f.write(content)

def create_redis_options(base_path, service_name):
    content = f'''namespace Pupitre.{service_name}.Infrastructure.Redis.Options;

public class RedisSyncOptions
{{
    public const string SectionName = "Redis:Sync";
    public bool Enabled {{ get; set; }} = true;
    public int SyncIntervalMs {{ get; set; }} = 60000;
    public int BatchSize {{ get; set; }} = 100;
}}
'''
    path = os.path.join(base_path, "Redis", "Options", "RedisSyncOptions.cs")
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with open(path, 'w') as f:
        f.write(content)

def create_composite_repository(base_path, service_name, entity_name):
    content = f'''using Microsoft.Extensions.Logging;
using Pupitre.{service_name}.Domain.Entities;
using Pupitre.{service_name}.Domain.Repositories;
using Pupitre.{service_name}.Infrastructure.EF.Repositories;
using Pupitre.{service_name}.Infrastructure.Mongo.Repositories;
using Pupitre.{service_name}.Infrastructure.Redis.Repositories;

namespace Pupitre.{service_name}.Infrastructure.Composite;

internal class Composite{entity_name}Repository : I{entity_name}Repository
{{
    private readonly {entity_name}PostgresRepository _postgresRepo;
    private readonly {entity_name}MongoRepository _mongoRepo;
    private readonly {entity_name}RedisRepository _redisRepo;
    private readonly ILogger<Composite{entity_name}Repository> _logger;

    public Composite{entity_name}Repository(
        {entity_name}PostgresRepository postgresRepo,
        {entity_name}MongoRepository mongoRepo,
        {entity_name}RedisRepository redisRepo,
        ILogger<Composite{entity_name}Repository> logger)
    {{
        _postgresRepo = postgresRepo;
        _mongoRepo = mongoRepo;
        _redisRepo = redisRepo;
        _logger = logger;
    }}

    public async Task AddAsync({entity_name} entity, CancellationToken cancellationToken = default)
    {{
        await _postgresRepo.AddAsync(entity, cancellationToken);
        await TryAsync(() => _redisRepo.AddAsync(entity, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.AddAsync(entity, cancellationToken), "MongoDB");
    }}

    public async Task<{entity_name}?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {{
        try {{ var r = await _redisRepo.GetAsync(id, cancellationToken); if (r != null) return r; }}
        catch {{ _logger.LogWarning("Redis read failed, trying MongoDB"); }}
        try {{ var m = await _mongoRepo.GetAsync(id, cancellationToken); if (m != null) return m; }}
        catch {{ _logger.LogWarning("MongoDB read failed, trying PostgreSQL"); }}
        return await _postgresRepo.GetAsync(id, cancellationToken);
    }}

    public async Task UpdateAsync({entity_name} entity, CancellationToken cancellationToken = default)
    {{
        await _postgresRepo.UpdateAsync(entity, cancellationToken);
        await TryAsync(() => _redisRepo.UpdateAsync(entity, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.UpdateAsync(entity, cancellationToken), "MongoDB");
    }}

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {{
        await _postgresRepo.DeleteAsync(id, cancellationToken);
        await TryAsync(() => _redisRepo.DeleteAsync(id, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.DeleteAsync(id, cancellationToken), "MongoDB");
    }}

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {{
        try {{ if (await _redisRepo.ExistsAsync(id, cancellationToken)) return true; }} catch {{ }}
        try {{ if (await _mongoRepo.ExistsAsync(id, cancellationToken)) return true; }} catch {{ }}
        return await _postgresRepo.ExistsAsync(id, cancellationToken);
    }}

    public Task<IReadOnlyList<{entity_name}>> BrowseAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.BrowseAsync(cancellationToken);

    private async Task TryAsync(Func<Task> action, string store)
    {{
        try {{ await action(); }}
        catch (Exception ex) {{ _logger.LogWarning(ex, "Propagation to {{Store}} failed", store); }}
    }}
}}
'''
    path = os.path.join(base_path, "Composite", f"Composite{entity_name}Repository.cs")
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with open(path, 'w') as f:
        f.write(content)

def create_composite_extensions(base_path, service_name, entity_name):
    content = f'''using Microsoft.Extensions.DependencyInjection;
using Mamey.Microservice.Infrastructure;
using Pupitre.{service_name}.Domain.Repositories;

namespace Pupitre.{service_name}.Infrastructure.Composite;

internal static class Extensions
{{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {{
        builder.Services.AddScoped<I{entity_name}Repository, Composite{entity_name}Repository>();
        return builder;
    }}
}}
'''
    path = os.path.join(base_path, "Composite", "Extensions.cs")
    with open(path, 'w') as f:
        f.write(content)

def add_persistence_layers(service_type, service_name, entity_name):
    base_dir = "/Volumes/Barracuda/mamey-io/code-final/Pupitre"
    base_path = os.path.join(base_dir, "src", "Services", service_type, f"Pupitre.{service_name}", "src", f"Pupitre.{service_name}.Infrastructure")
    
    # Create directories
    for subdir in ["Redis/Repositories", "Redis/Services", "Redis/Options", "Composite"]:
        os.makedirs(os.path.join(base_path, subdir), exist_ok=True)
    
    # Create files
    create_redis_repository(base_path, service_name, entity_name)
    create_redis_extensions(base_path, service_name, entity_name)
    create_redis_sync_service(base_path, service_name, entity_name)
    create_redis_options(base_path, service_name)
    create_composite_repository(base_path, service_name, entity_name)
    create_composite_extensions(base_path, service_name, entity_name)
    
    print(f"Created persistence layers for Pupitre.{service_name}")

if __name__ == "__main__":
    # Foundation services
    foundation = [
        ("Users", "User"),
        ("GLEs", "GLE"),
        ("Curricula", "Curriculum"),
        ("Lessons", "Lesson"),
        ("Assessments", "Assessment"),
        ("Progress", "LearningProgress"),
        ("Rewards", "Reward"),
        ("Notifications", "Notification"),
        ("Parents", "Parent"),
        ("Analytics", "Analytic"),
    ]
    
    # AI services
    ai = [
        ("AITutors", "Tutor"),
        ("AIAssessments", "AIAssessment"),
        ("AIContent", "ContentGeneration"),
        ("AISpeech", "SpeechRequest"),
        ("AIAdaptive", "AdaptiveLearning"),
        ("AIBehavior", "Behavior"),
        ("AISafety", "SafetyCheck"),
        ("AIRecommendations", "AIRecommendation"),
        ("AITranslation", "TranslationRequest"),
        ("AIVision", "VisionAnalysis"),
    ]
    
    # Support services
    support = [
        ("Educators", "Educator"),
        ("Fundraising", "Campaign"),
        ("Bookstore", "Book"),
        ("Aftercare", "AftercarePlan"),
        ("Accessibility", "AccessProfile"),
        ("Compliance", "ComplianceRecord"),
        ("Ministries", "MinistryData"),
        ("Operations", "OperationMetric"),
    ]
    
    for svc, entity in foundation:
        add_persistence_layers("Foundation", svc, entity)
    
    for svc, entity in ai:
        add_persistence_layers("AI", svc, entity)
    
    for svc, entity in support:
        add_persistence_layers("Support", svc, entity)
    
    print("\nDone! Created persistence layers for all 28 services.")
