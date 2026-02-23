using Mamey.Auth.Decentralized.Core;

namespace Mamey.Auth.Decentralized.Methods;

/// <summary>
/// Interface for DID method implementations
/// </summary>
public interface IDidMethod
{
    /// <summary>
    /// Gets the name of the DID method
    /// </summary>
    string MethodName { get; }
    
    /// <summary>
    /// Gets the version of the DID method
    /// </summary>
    string Version { get; }
    
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
    /// Creates a new DID with the specified identifier
    /// </summary>
    /// <param name="identifier">The method-specific identifier</param>
    /// <param name="didDocument">The DID Document to associate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created DID</returns>
    Task<Did> CreateAsync(string identifier, DidDocument didDocument, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing DID Document
    /// </summary>
    /// <param name="did">The DID to update</param>
    /// <param name="didDocument">The updated DID Document</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the update was successful</returns>
    Task<bool> UpdateAsync(string did, DidDocument didDocument, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deactivates a DID
    /// </summary>
    /// <param name="did">The DID to deactivate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the deactivation was successful</returns>
    Task<bool> DeactivateAsync(string did, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Validates a DID identifier for this method
    /// </summary>
    /// <param name="identifier">The identifier to validate</param>
    /// <returns>True if the identifier is valid for this method</returns>
    bool ValidateIdentifier(string identifier);
}
