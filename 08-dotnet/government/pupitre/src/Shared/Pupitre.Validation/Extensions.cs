using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Pupitre.Validation;

public static class Extensions
{
    /// <summary>
    /// Registers all validators from the specified assembly.
    /// </summary>
    public static IServiceCollection AddValidatorsFromAssembly(
        this IServiceCollection services,
        Assembly assembly)
    {
        var validatorTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>))
                .Select(i => new { Interface = i, Implementation = t }));

        foreach (var validator in validatorTypes)
        {
            services.AddScoped(validator.Interface, validator.Implementation);
        }

        return services;
    }

    /// <summary>
    /// Registers all validators from the calling assembly.
    /// </summary>
    public static IServiceCollection AddValidatorsFromCallingAssembly(this IServiceCollection services)
    {
        return services.AddValidatorsFromAssembly(Assembly.GetCallingAssembly());
    }
}
