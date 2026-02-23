using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Compliance.Infrastructure.Redis.Repositories;
using Pupitre.Compliance.Infrastructure.Redis.Services;

namespace Pupitre.Compliance.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<ComplianceRecordRedisRepository>();
        builder.Services.AddHostedService<ComplianceRecordRedisSyncService>();
        return builder;
    }
}
