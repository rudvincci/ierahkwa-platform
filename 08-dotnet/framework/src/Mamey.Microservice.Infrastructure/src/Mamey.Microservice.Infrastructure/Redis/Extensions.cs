using Mamey.Persistence.Redis;

using StackExchange.Redis;

namespace Mamey.Microservice.Infrastructure.Redis
{
    internal static class Extensions
    {
        public static IServiceCollection AddRedis(this IServiceCollection services, RedisOptions options)
        {
            services
            .AddSingleton(options)
            .AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(options.ConnectionString))
            .AddTransient(sp => sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase(options.Database))
            .AddStackExchangeRedisCache(o =>
            {
                o.Configuration = options.ConnectionString;
                o.InstanceName = options.Instance;
            });

            return services;
        }
    }
}

