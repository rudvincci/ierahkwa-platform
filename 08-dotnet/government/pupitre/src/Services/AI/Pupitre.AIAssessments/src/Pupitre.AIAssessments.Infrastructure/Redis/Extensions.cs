using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AIAssessments.Infrastructure.Redis.Repositories;
using Pupitre.AIAssessments.Infrastructure.Redis.Services;

namespace Pupitre.AIAssessments.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<AIAssessmentRedisRepository>();
        builder.Services.AddHostedService<AIAssessmentRedisSyncService>();
        return builder;
    }
}
