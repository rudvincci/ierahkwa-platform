using Mamey.Persistence.Redis.Builders;
using Mamey.Streaming;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Mamey.Persistence.Redis;

public static class Extensions
{
    private const string SectionName = "redis";
    private const string RegistryName = "persistence.redis";

    public static IMameyBuilder AddRedis(this IMameyBuilder builder, string sectionName = SectionName)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        var options = builder.GetOptions<RedisOptions>(sectionName);
        return builder.AddRedis(options);
    }

    public static IMameyBuilder AddRedis(this IMameyBuilder builder,
        Func<IRedisOptionsBuilder, IRedisOptionsBuilder> buildOptions)
    {
        var options = buildOptions(new RedisOptionsBuilder()).Build();
        return builder.AddRedis(options);
    }

    public static IMameyBuilder AddRedis(this IMameyBuilder builder, RedisOptions options)
    {
        if (!builder.TryRegister(RegistryName))
        {
            return builder;
        }

        builder.Services
            .AddSingleton(options)
            .AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(options.ConnectionString))
            .AddTransient<IDatabase>(sp => sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase(options.Database))
            .AddSingleton<ICache>(sp => new RedisCache(sp.GetRequiredService<IDatabase>()))
            .AddStackExchangeRedisCache(o =>
            {
                o.Configuration = options.ConnectionString;
                o.InstanceName = options.Instance;
            });

        return builder;
    }
    
    public static IServiceCollection AddRedisStreaming(this IServiceCollection services)
        => services
            .AddSingleton<IStreamPublisher, RedisStreamPublisher>()
            .AddSingleton<IStreamSubscriber, RedisStreamSubscriber>();
}