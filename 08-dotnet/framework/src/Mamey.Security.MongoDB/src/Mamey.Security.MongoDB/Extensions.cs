using System.Reflection;
using Mamey.Security.MongoDB.Serializers;
using Mamey.Types;
using MongoDB.Bson.Serialization;

namespace Mamey.Security.MongoDB;

/// <summary>
/// Extension methods for registering MongoDB security serializers.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds MongoDB security serializers to the Mamey builder.
    /// This enables automatic encryption and hashing for properties marked with [EncryptedAttribute] and [HashedAttribute].
    /// </summary>
    /// <param name="builder">The IMameyBuilder instance.</param>
    /// <returns>The IMameyBuilder instance for chaining.</returns>
    public static IMameyBuilder AddSecurityMongoDB(this IMameyBuilder builder)
    {
        if (builder == null)
            throw new NullReferenceException(nameof(builder));
        // MongoDB serializers are registered via convention
        // No services need to be registered here, just return the builder
        return builder;
    }

    /// <summary>
    /// Registers MongoDB serializers for all types in the specified assemblies that have properties
    /// marked with [EncryptedAttribute] or [HashedAttribute].
    /// </summary>
    /// <param name="securityProvider">The ISecurityProvider instance.</param>
    /// <param name="assemblies">The assemblies to scan for types with security attributes.</param>
    public static void RegisterSecuritySerializers(ISecurityProvider securityProvider, params Assembly[] assemblies)
    {
        if (securityProvider == null)
            throw new ArgumentNullException(nameof(securityProvider));
        if (assemblies == null || assemblies.Length == 0)
            return;

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract);

            foreach (var type in types)
            {
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.PropertyType == typeof(string) && 
                                (p.GetCustomAttribute<EncryptedAttribute>() != null ||
                                 p.GetCustomAttribute<HashedAttribute>() != null));

                if (!properties.Any())
                    continue;

                if (!BsonClassMap.IsClassMapRegistered(type))
                {
                    var classMap = new BsonClassMap(type);
                    classMap.AutoMap();
                    foreach (var property in properties)
                    {
                        var memberMap = classMap.GetMemberMap(property.Name);
                        if (memberMap != null)
                        {
                            if (property.GetCustomAttribute<EncryptedAttribute>() != null)
                            {
                                memberMap.SetSerializer(new EncryptedStringSerializer(securityProvider));
                            }
                            else if (property.GetCustomAttribute<HashedAttribute>() != null)
                            {
                                memberMap.SetSerializer(new HashedStringSerializer(securityProvider));
                            }
                        }
                    }
                    BsonClassMap.RegisterClassMap(classMap);
                }
            }
        }
    }
}

