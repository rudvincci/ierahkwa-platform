#!/usr/bin/env python3
"""
Add MongoDB sync services to Pupitre microservices.
These services sync data from PostgreSQL to MongoDB for fast reads.
"""

import os

def create_mongo_sync_service(base_path, service_name, entity_name):
    content = f'''using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pupitre.{service_name}.Infrastructure.EF.Repositories;
using Pupitre.{service_name}.Infrastructure.Mongo.Options;
using Pupitre.{service_name}.Infrastructure.Mongo.Repositories;

namespace Pupitre.{service_name}.Infrastructure.Mongo.Services;

/// <summary>
/// Background service that syncs data from PostgreSQL to MongoDB.
/// </summary>
internal class {entity_name}MongoSyncService : BackgroundService
{{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<{entity_name}MongoSyncService> _logger;
    private readonly MongoSyncOptions _options;

    public {entity_name}MongoSyncService(
        IServiceProvider serviceProvider,
        ILogger<{entity_name}MongoSyncService> logger,
        IOptions<MongoSyncOptions> options)
    {{
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }}

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {{
        if (!_options.Enabled)
        {{
            _logger.LogInformation("MongoDB sync service is disabled");
            return;
        }}

        _logger.LogInformation("MongoDB sync service starting with interval: {{Interval}}ms", _options.SyncIntervalMs);

        while (!stoppingToken.IsCancellationRequested)
        {{
            try
            {{
                await SyncAsync(stoppingToken);
            }}
            catch (Exception ex)
            {{
                _logger.LogError(ex, "Error during MongoDB sync");
            }}

            await Task.Delay(_options.SyncIntervalMs, stoppingToken);
        }}
    }}

    private async Task SyncAsync(CancellationToken cancellationToken)
    {{
        using var scope = _serviceProvider.CreateScope();
        var postgresRepo = scope.ServiceProvider.GetRequiredService<{entity_name}PostgresRepository>();
        var mongoRepo = scope.ServiceProvider.GetRequiredService<{entity_name}MongoRepository>();

        var items = await postgresRepo.BrowseAsync(cancellationToken);
        var syncCount = 0;

        foreach (var item in items)
        {{
            try
            {{
                // Check if exists, update if yes, add if no
                if (await mongoRepo.ExistsAsync(item.Id, cancellationToken))
                {{
                    await mongoRepo.UpdateAsync(item, cancellationToken);
                }}
                else
                {{
                    await mongoRepo.AddAsync(item, cancellationToken);
                }}
                syncCount++;
            }}
            catch (Exception ex)
            {{
                _logger.LogWarning(ex, "Failed to sync item {{Id}} to MongoDB", item.Id);
            }}
        }}

        _logger.LogDebug("Synced {{Count}} items to MongoDB", syncCount);
    }}
}}
'''
    path = os.path.join(base_path, "Mongo", "Services", f"{entity_name}MongoSyncService.cs")
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with open(path, 'w') as f:
        f.write(content)

def create_mongo_sync_options(base_path, service_name):
    content = f'''namespace Pupitre.{service_name}.Infrastructure.Mongo.Options;

public class MongoSyncOptions
{{
    public const string SectionName = "Mongo:Sync";
    public bool Enabled {{ get; set; }} = true;
    public int SyncIntervalMs {{ get; set; }} = 60000;
    public int BatchSize {{ get; set; }} = 100;
}}
'''
    path = os.path.join(base_path, "Mongo", "Options", "MongoSyncOptions.cs")
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with open(path, 'w') as f:
        f.write(content)

def update_mongo_extensions(base_path, service_name, entity_name):
    """Update Mongo Extensions to register sync service"""
    ext_path = os.path.join(base_path, "Mongo", "Extensions.cs")
    if os.path.exists(ext_path):
        with open(ext_path, 'r') as f:
            content = f.read()
        
        # Check if already has sync service
        if "MongoSyncService" in content:
            return
        
        # Add using for Services namespace if not present
        services_using = f"using Pupitre.{service_name}.Infrastructure.Mongo.Services;"
        if services_using not in content:
            # Add after last using
            lines = content.split('\n')
            for i, line in enumerate(lines):
                if line.strip().startswith('namespace'):
                    lines.insert(i, services_using)
                    break
            content = '\n'.join(lines)
        
        # Add sync service registration
        if "AddHostedService" not in content:
            content = content.replace(
                "return builder;",
                f"builder.Services.AddHostedService<{entity_name}MongoSyncService>();\n        return builder;"
            )
        
        with open(ext_path, 'w') as f:
            f.write(content)

def add_mongo_sync(service_type, service_name, entity_name):
    base_dir = "/Volumes/Barracuda/mamey-io/code-final/Pupitre"
    base_path = os.path.join(base_dir, "src", "Services", service_type, f"Pupitre.{service_name}", "src", f"Pupitre.{service_name}.Infrastructure")
    
    # Create directories
    os.makedirs(os.path.join(base_path, "Mongo", "Services"), exist_ok=True)
    os.makedirs(os.path.join(base_path, "Mongo", "Options"), exist_ok=True)
    
    # Create files
    create_mongo_sync_service(base_path, service_name, entity_name)
    create_mongo_sync_options(base_path, service_name)
    update_mongo_extensions(base_path, service_name, entity_name)
    
    print(f"Created Mongo sync service for Pupitre.{service_name}")

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
        add_mongo_sync("Foundation", svc, entity)
    
    for svc, entity in ai:
        add_mongo_sync("AI", svc, entity)
    
    for svc, entity in support:
        add_mongo_sync("Support", svc, entity)
    
    print("\nDone! Created Mongo sync services for all 28 services.")
