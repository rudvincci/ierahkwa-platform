using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Bookstore.Infrastructure.Redis.Repositories;
using Pupitre.Bookstore.Infrastructure.Redis.Services;

namespace Pupitre.Bookstore.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<BookRedisRepository>();
        builder.Services.AddHostedService<BookRedisSyncService>();
        return builder;
    }
}
