namespace Mamey.Identity.Decentralized.Abstractions;

/// <summary>
/// Describes a service endpoint in a DID Document.
/// </summary>
public interface IDidServiceEndpoint
{
    /// <summary>
    /// The unique identifier for the service endpoint (usually a fragment).
    /// </summary>
    string Id { get; }

    /// <summary>
    /// The service type (e.g., "LinkedDomains", "MessagingService").
    /// </summary>
    string Type { get; }

    /// <summary>
    /// One or more service endpoint URIs or JSON objects.
    /// </summary>
    IReadOnlyList<object> Endpoints { get; }

    /// <summary>
    /// Additional extension properties.
    /// </summary>
    IReadOnlyDictionary<string, object> AdditionalProperties { get; }
}