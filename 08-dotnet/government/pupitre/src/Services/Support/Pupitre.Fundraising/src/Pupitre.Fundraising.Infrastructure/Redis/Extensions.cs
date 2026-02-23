using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Fundraising.Infrastructure.Redis.Repositories;
using Pupitre.Fundraising.Infrastructure.Redis.Services;

namespace Pupitre.Fundraising.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<CampaignRedisRepository>();
        builder.Services.AddHostedService<CampaignRedisSyncService>();
        return builder;
    }
}
