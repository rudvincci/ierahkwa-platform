using Mamey.Auth.DecentralizedIdentifiers.Abstractions;

namespace Mamey.Auth.DecentralizedIdentifiers;

/// <summary>
/// Extension methods for the method registry to query or enumerate support.
/// </summary>
public static class RegistryExtensions
{
    /// <summary>
    /// Checks if a registry supports a DID method.
    /// </summary>
    public static bool Supports(this IDidMethodRegistry registry, string methodName)
    {
        if (registry == null) throw new ArgumentNullException(nameof(registry));
        return registry.Get(methodName) != null;
    }
}