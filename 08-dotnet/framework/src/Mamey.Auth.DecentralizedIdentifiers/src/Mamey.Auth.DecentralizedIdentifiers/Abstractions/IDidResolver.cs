using Mamey.Auth.DecentralizedIdentifiers.Core;

namespace Mamey.Auth.DecentralizedIdentifiers.Abstractions;

/// <summary>
/// Defines the contract for resolving DIDs to DID Documents according to the W3C DID Resolution specification.
/// </summary>
public interface IDidResolver
{
    /// <summary>
    /// Resolves a Decentralized Identifier (DID) to a DID Document.
    /// </summary>
    /// <param name="did">The DID to resolve (e.g., "did:example:123456").</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task resulting in a DID Document resolution result.</returns>
    Task<DidResolutionResult> ResolveAsync(string did, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns true if the resolver supports the provided DID method.
    /// </summary>
    /// <param name="didMethod">The DID method name (e.g., "key", "web", "ion").</param>
    /// <returns>True if the resolver supports the DID method; otherwise, false.</returns>
    /// <remarks>
    /// This allows the resolver to act as a dispatcher for different DID methods (e.g., key, web, ion, pkh).
    /// </remarks>
    bool SupportsMethod(string didMethod);
}