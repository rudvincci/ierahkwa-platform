using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Operations.Infrastructure.Redis.Repositories;
using Pupitre.Operations.Infrastructure.Redis.Services;

namespace Pupitre.Operations.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<OperationMetricRedisRepository>();
        builder.Services.AddHostedService<OperationMetricRedisSyncService>();
        return builder;
    }
}
