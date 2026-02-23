using Mamey.Auth.DecentralizedIdentifiers.Abstractions;

namespace Mamey.Auth.DecentralizedIdentifiers;

/// <summary>
/// Provides extension methods for IDidMethod, such as dynamic registration, metadata decoration, etc.
/// </summary>
public static class DidMethodExtension
{
    /// <summary>
    /// Registers a method with a registry, supporting fluent chaining.
    /// </summary>
    public static IDidMethodRegistry RegisterMethod(this IDidMethodRegistry registry, IDidMethod method)
    {
        if (registry == null) throw new ArgumentNullException(nameof(registry));
        if (method == null) throw new ArgumentNullException(nameof(method));
        registry.Register(method);
        return registry;
    }
}