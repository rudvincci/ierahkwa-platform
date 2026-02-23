using Mamey.Auth.DecentralizedIdentifiers.Core;

namespace Mamey.Auth.DecentralizedIdentifiers.Abstractions;

/// <summary>
/// Resolves or dereferences DID URLs (fragments, services, keys) according to the W3C DID URL Dereferencing specification.
/// </summary>
public interface IDidDereferencer
{
    /// <summary>
    /// Dereferences a DID URL (e.g., fragment, verification method, service endpoint).
    /// </summary>
    /// <param name="didUrl">The full DID URL to dereference.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A dereferencing result containing the target resource or information.</returns>
    Task<DidDereferencingResult> DereferenceAsync(string didUrl, CancellationToken cancellationToken = default);
}