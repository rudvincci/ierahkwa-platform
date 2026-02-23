using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Government.Identity.Infrastructure.EF.Repositories;
using Mamey.Government.Identity.Infrastructure.Redis.Options;
using Mamey.Government.Identity.Infrastructure.Redis.Repositories;
using Mamey.Government.Identity.Infrastructure.Redis.Services;

namespace Mamey.Government.Identity.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder, string sectionName = "redisSync")
    {
        // Register Redis sync options
        builder.Services.Configure<RedisSyncOptions>(builder.Configuration.GetSection(sectionName));
        // ICache is already registered by AddRedis() called elsewhere
        builder.Services.AddScoped<SessionRedisRepository>();
        builder.Services.AddScoped<SessionPostgresRepository>();
        builder.Services.AddScoped<ISessionRepository, CompositeSessionRepository>();
        
        // Register Redis repositories for composite repos
        builder.Services.AddScoped<UserRedisRepository>();
        builder.Services.AddScoped<RoleRedisRepository>();
        builder.Services.AddScoped<SubjectRedisRepository>();
        builder.Services.AddScoped<PermissionRedisRepository>();
        builder.Services.AddScoped<EmailConfirmationRedisRepository>();
        builder.Services.AddScoped<TwoFactorAuthRedisRepository>();
        builder.Services.AddScoped<MultiFactorAuthRedisRepository>();
        builder.Services.AddScoped<MfaChallengeRedisRepository>();
        builder.Services.AddScoped<CredentialRedisRepository>();
        
        // Register Postgres repositories needed by composite repos and sync services
        builder.Services.AddScoped<UserPostgresRepository>();
        builder.Services.AddScoped<SubjectPostgresRepository>();
        builder.Services.AddScoped<RolePostgresRepository>();
        builder.Services.AddScoped<PermissionPostgresRepository>();
        builder.Services.AddScoped<EmailConfirmationPostgresRepository>();
        builder.Services.AddScoped<TwoFactorAuthPostgresRepository>();
        builder.Services.AddScoped<MultiFactorAuthPostgresRepository>();
        builder.Services.AddScoped<MfaChallengePostgresRepository>();
        builder.Services.AddScoped<CredentialPostgresRepository>();
        
        // Register background sync services to sync from Postgres to Redis
        builder.Services.AddHostedService<UserRedisSyncService>();
        builder.Services.AddHostedService<SubjectRedisSyncService>();
        builder.Services.AddHostedService<RoleRedisSyncService>();
        builder.Services.AddHostedService<PermissionRedisSyncService>();
        builder.Services.AddHostedService<CredentialRedisSyncService>();
        builder.Services.AddHostedService<EmailConfirmationRedisSyncService>();
        builder.Services.AddHostedService<TwoFactorAuthRedisSyncService>();
        builder.Services.AddHostedService<MultiFactorAuthRedisSyncService>();
        builder.Services.AddHostedService<MfaChallengeRedisSyncService>();
        
        return builder;
    }
}
