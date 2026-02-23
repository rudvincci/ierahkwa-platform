using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Mamey.Security.EntityFramework.ValueConverters;

namespace Mamey.Security.EntityFramework;

/// <summary>
/// Extension methods for ModelBuilder to automatically apply encryption and hashing converters
/// based on [EncryptedAttribute] and [HashedAttribute] attributes.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Automatically applies encryption and hashing converters to properties marked with
    /// [EncryptedAttribute] and [HashedAttribute] attributes.
    /// </summary>
    /// <param name="modelBuilder">The ModelBuilder instance.</param>
    /// <param name="securityProvider">The ISecurityProvider instance for encryption/hashing operations.</param>
    /// <returns>The ModelBuilder instance for chaining.</returns>
    public static ModelBuilder ApplySecurityAttributes(this ModelBuilder modelBuilder, ISecurityProvider securityProvider)
    {
        if (modelBuilder == null)
            throw new ArgumentNullException(nameof(modelBuilder));
        if (securityProvider == null)
            throw new ArgumentNullException(nameof(securityProvider));

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType != typeof(string))
                    continue;

                var memberInfo = property.PropertyInfo as MemberInfo ?? property.FieldInfo as MemberInfo;
                if (memberInfo == null)
                    continue;

                // Check for EncryptedAttribute
                if (memberInfo.GetCustomAttributes(typeof(EncryptedAttribute), inherit: true).Any())
                {
                    property.SetValueConverter(new EncryptedValueConverter(securityProvider));
                }
                // Check for HashedAttribute
                else if (memberInfo.GetCustomAttributes(typeof(HashedAttribute), inherit: true).Any())
                {
                    property.SetValueConverter(new HashedValueConverter(securityProvider));
                }
            }
        }

        return modelBuilder;
    }

    /// <summary>
    /// Automatically applies encryption and hashing converters to properties marked with
    /// [EncryptedAttribute] and [HashedAttribute] attributes using a service provider.
    /// </summary>
    /// <param name="modelBuilder">The ModelBuilder instance.</param>
    /// <param name="serviceProvider">The service provider to resolve ISecurityProvider.</param>
    /// <returns>The ModelBuilder instance for chaining.</returns>
    public static ModelBuilder ApplySecurityAttributes(this ModelBuilder modelBuilder, IServiceProvider serviceProvider)
    {
        if (modelBuilder == null)
            throw new ArgumentNullException(nameof(modelBuilder));
        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        var securityProvider = serviceProvider.GetRequiredService<ISecurityProvider>();
        return modelBuilder.ApplySecurityAttributes(securityProvider);
    }
}

