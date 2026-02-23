using Mamey.Auth.DecentralizedIdentifiers.Abstractions;

namespace Mamey.Auth.DecentralizedIdentifiers.Validation;

/// <summary>
/// Validates DID Document service endpoints.
/// </summary>
public static class ServiceEndpointValidator
{
    /// <summary>
    /// Validates the service endpoint entry.
    /// </summary>
    public static IList<string> Validate(IDidServiceEndpoint service)
    {
        var warnings = new List<string>();
        if (service == null) throw new ArgumentNullException(nameof(service));
        if (string.IsNullOrWhiteSpace(service.Id))
            throw new ArgumentException("Service endpoint missing 'id'.");
        if (string.IsNullOrWhiteSpace(service.Type))
            warnings.Add($"Service endpoint '{service.Id}' is missing 'type'.");

        if (service.Endpoints == null || service.Endpoints.Count == 0)
            warnings.Add($"Service endpoint '{service.Id}' does not specify any endpoint value.");

        // Could add URL format check or JSON schema validation for endpoint objects
        return warnings;
    }
}