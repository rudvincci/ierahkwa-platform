using Mamey.Identity.Decentralized.Abstractions;

namespace Mamey.Identity.Decentralized;

/// <summary>
/// Extension methods for IDidServiceEndpoint, for fast-lookup and type conversions.
/// </summary>
public static class ServiceEndpointExtensions
{
    /// <summary>
    /// Determines if the endpoint is of a specified type.
    /// </summary>
    public static bool IsType(this IDidServiceEndpoint service, string type)
    {
        if (service == null) throw new ArgumentNullException(nameof(service));
        return string.Equals(service.Type, type, StringComparison.OrdinalIgnoreCase);
    }
}

