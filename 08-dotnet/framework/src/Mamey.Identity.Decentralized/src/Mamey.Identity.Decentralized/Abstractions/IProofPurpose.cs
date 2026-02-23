namespace Mamey.Identity.Decentralized.Abstractions;

/// <summary>
/// Describes the purpose of a proof as defined by the W3C Verifiable Credentials and DID specs.
/// </summary>
public interface IProofPurpose
{
    /// <summary>
    /// The type of proof purpose (e.g., "authentication", "assertionMethod").
    /// </summary>
    string Purpose { get; }
}