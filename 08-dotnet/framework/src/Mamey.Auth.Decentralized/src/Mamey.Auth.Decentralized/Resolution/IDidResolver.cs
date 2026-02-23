using Mamey.Auth.Decentralized.Core;

namespace Mamey.Auth.Decentralized.Resolution;

/// <summary>
/// Interface for DID resolution operations
/// </summary>
public interface IDidResolver
{
    /// <summary>
    /// Resolves a DID to its DID Document
    /// </summary>
    /// <param name="did">The DID to resolve</param>
    /// <param name="options">Optional resolution options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The resolution result</returns>
    Task<DidResolutionResult> ResolveAsync(string did, DidResolutionOptions? options = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Dereferences a DID URL to its content
    /// </summary>
    /// <param name="didUrl">The DID URL to dereference</param>
    /// <param name="options">Optional dereferencing options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The dereferencing result</returns>
    Task<DidDereferencingResult> DereferenceAsync(string didUrl, DidDereferencingOptions? options = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the supported DID methods
    /// </summary>
    /// <returns>List of supported DID methods</returns>
    IEnumerable<string> GetSupportedMethods();
    
    /// <summary>
    /// Checks if a DID method is supported
    /// </summary>
    /// <param name="method">The DID method to check</param>
    /// <returns>True if the method is supported</returns>
    bool IsMethodSupported(string method);
}
