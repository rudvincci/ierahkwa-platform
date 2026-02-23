using Mamey.Security.Redis.Serializers;
using Mamey.Types;
using Microsoft.Extensions.DependencyInjection;
using Mamey.Security;

namespace Mamey.Security.Redis;

/// <summary>
/// Extension methods for registering Redis security serializers.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds Redis security serializers to the Mamey builder.
    /// This enables automatic encryption and hashing for Redis operations.
    /// </summary>
    /// <param name="builder">The IMameyBuilder instance.</param>
    /// <returns>The IMameyBuilder instance for chaining.</returns>
    public static IMameyBuilder AddSecurityRedis(this IMameyBuilder builder)
    {
        // Check if ISecurityProvider is registered (requires AddSecurity to be called first)
        using (var tempProvider = builder.Services.BuildServiceProvider())
        {
            var securityProvider = tempProvider.GetService<ISecurityProvider>();
            if (securityProvider == null)
            {
                throw new InvalidOperationException("ISecurityProvider is not registered. Call AddSecurity() before AddSecurityRedis().");
            }
        }

        // Register serializers as singletons
        builder.Services.AddSingleton<EncryptedRedisSerializer>(sp =>
        {
            var securityProvider = sp.GetRequiredService<ISecurityProvider>();
            return new EncryptedRedisSerializer(securityProvider);
        });

        builder.Services.AddSingleton<HashedRedisSerializer>(sp =>
        {
            var securityProvider = sp.GetRequiredService<ISecurityProvider>();
            return new HashedRedisSerializer(securityProvider);
        });

        return builder;
    }
}

