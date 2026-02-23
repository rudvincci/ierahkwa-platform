namespace Mamey.Auth.DecentralizedIdentifiers.Abstractions;

/// <summary>
/// Interface for custom DID method implementations, supporting resolution and (optionally) CRUD operations.
/// </summary>
public interface IDidMethod
{
    /// <summary>
    /// Gets the canonical method name (e.g., "key", "web", "ion", etc.).
    /// This is used for lookup and registry purposes.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Resolves a DID to its corresponding DID Document.
    /// </summary>
    /// <param name="did">The DID to resolve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// A task yielding the resolved DID Document.
    /// Throws an exception if the DID is invalid or cannot be resolved.
    /// </returns>
    Task<IDidDocument> ResolveAsync(string did, CancellationToken cancellationToken = default);

    /// <summary>
    /// (Optional) Creates a new DID Document using the method-specific options.
    /// If not supported by a given method, should throw NotSupportedException.
    /// </summary>
    /// <param name="options">Creation options as an object or strongly-typed per method.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// The newly created DID Document.
    /// Throws NotSupportedException if creation is not supported for this method.
    /// </returns>
    Task<IDidDocument> CreateAsync(object options, CancellationToken cancellationToken = default);

    /// <summary>
    /// (Optional) Updates an existing DID Document.
    /// If not supported by a given method, should throw NotSupportedException.
    /// </summary>
    /// <param name="did">The DID to update.</param>
    /// <param name="updateRequest">Update request data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// The updated DID Document.
    /// Throws NotSupportedException if update is not supported for this method.
    /// </returns>
    Task<IDidDocument> UpdateAsync(string did, object updateRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// (Optional) Deactivates the specified DID.
    /// If not supported by a given method, should throw NotSupportedException.
    /// </summary>
    /// <param name="did">The DID to deactivate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// A task indicating completion.
    /// Throws NotSupportedException if deactivation is not supported for this method.
    /// </returns>
    Task DeactivateAsync(string did, CancellationToken cancellationToken = default);
}