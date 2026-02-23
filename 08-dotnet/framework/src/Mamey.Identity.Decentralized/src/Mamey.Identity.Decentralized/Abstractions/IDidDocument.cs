namespace Mamey.Identity.Decentralized.Abstractions;

/// <summary>
/// Represents a W3C-compliant Decentralized Identifier (DID) Document.
/// </summary>
public interface IDidDocument
{
    /// <summary>
    /// The JSON-LD context for the DID Document.
    /// </summary>
    IReadOnlyList<string> Context { get; }

    /// <summary>
    /// The identifier for the DID Document (e.g., "did:example:123456").
    /// </summary>
    string Id { get; }

    /// <summary>
    /// The controller or list of controllers for the DID.
    /// </summary>
    IReadOnlyList<string> Controller { get; }

    /// <summary>
    /// Collection of verification methods declared in this document.
    /// </summary>
    IReadOnlyList<IDidVerificationMethod> VerificationMethods { get; }

    /// <summary>
    /// Collection of verification method references or embedded verification methods used for authentication.
    /// </summary>
    IReadOnlyList<object> Authentication { get; }

    /// <summary>
    /// Collection for assertionMethod verification relationships.
    /// </summary>
    IReadOnlyList<object> AssertionMethod { get; }

    /// <summary>
    /// Collection for keyAgreement verification relationships.
    /// </summary>
    IReadOnlyList<object> KeyAgreement { get; }

    /// <summary>
    /// Collection for capabilityDelegation verification relationships.
    /// </summary>
    IReadOnlyList<object> CapabilityDelegation { get; }

    /// <summary>
    /// Collection for capabilityInvocation verification relationships.
    /// </summary>
    IReadOnlyList<object> CapabilityInvocation { get; }

    /// <summary>
    /// Service endpoints defined in this DID Document.
    /// </summary>
    IReadOnlyList<IDidServiceEndpoint> ServiceEndpoints { get; }

    /// <summary>
    /// Additional properties for custom extension support.
    /// </summary>
    IReadOnlyDictionary<string, object> AdditionalProperties { get; }
}