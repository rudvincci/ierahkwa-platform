using Mamey.Types;

namespace Mamey.Security.EntityFramework;

/// <summary>
/// Extension methods for registering Entity Framework Core security features.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds Entity Framework Core security support to the Mamey builder.
    /// This enables automatic encryption and hashing for properties marked with [EncryptedAttribute] and [HashedAttribute].
    /// </summary>
    /// <param name="builder">The IMameyBuilder instance.</param>
    /// <returns>The IMameyBuilder instance for chaining.</returns>
    public static IMameyBuilder AddSecurityEntityFramework(this IMameyBuilder builder)
    {
        // Entity Framework Core security is applied via ModelBuilder extensions
        // No services need to be registered here, just return the builder
        return builder;
    }
}



