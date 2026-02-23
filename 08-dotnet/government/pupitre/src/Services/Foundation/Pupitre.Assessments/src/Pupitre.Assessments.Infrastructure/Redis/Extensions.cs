using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Assessments.Infrastructure.Redis.Repositories;
using Pupitre.Assessments.Infrastructure.Redis.Services;

namespace Pupitre.Assessments.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<AssessmentRedisRepository>();
        builder.Services.AddHostedService<AssessmentRedisSyncService>();
        return builder;
    }
}
