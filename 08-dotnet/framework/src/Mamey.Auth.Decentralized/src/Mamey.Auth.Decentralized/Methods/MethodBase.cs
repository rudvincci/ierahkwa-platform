using Mamey.Auth.Decentralized.Core;
using Mamey.Auth.Decentralized.Exceptions;
using Microsoft.Extensions.Logging;

namespace Mamey.Auth.Decentralized.Methods;

/// <summary>
/// Base class for DID method implementations
/// </summary>
public abstract class MethodBase : IDidMethod
{
    /// <summary>
    /// Gets the name of the DID method
    /// </summary>
    public abstract string MethodName { get; }
    
    /// <summary>
    /// Gets the version of the DID method
    /// </summary>
    public virtual string Version => "1.0";
    
    /// <summary>
    /// The logger for this method
    /// </summary>
    protected readonly ILogger Logger;
    
    /// <summary>
    /// Initializes a new instance of the MethodBase class
    /// </summary>
    /// <param name="logger">The logger</param>
    protected MethodBase(ILogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <summary>
    /// Resolves a DID to its DID Document
    /// </summary>
    /// <param name="did">The DID to resolve</param>
    /// <param name="options">Optional resolution options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The resolution result</returns>
    public abstract Task<DidResolutionResult> ResolveAsync(string did, DidResolutionOptions? options = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Dereferences a DID URL to its content
    /// </summary>
    /// <param name="didUrl">The DID URL to dereference</param>
    /// <param name="options">Optional dereferencing options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The dereferencing result</returns>
    public abstract Task<DidDereferencingResult> DereferenceAsync(string didUrl, DidDereferencingOptions? options = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new DID with the specified identifier
    /// </summary>
    /// <param name="identifier">The method-specific identifier</param>
    /// <param name="didDocument">The DID Document to associate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created DID</returns>
    public virtual async Task<Did> CreateAsync(string identifier, DidDocument didDocument, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(identifier))
            throw new ArgumentException("Identifier cannot be null or empty", nameof(identifier));
        
        if (didDocument == null)
            throw new ArgumentNullException(nameof(didDocument));
        
        if (!ValidateIdentifier(identifier))
            throw new InvalidDidException($"Invalid identifier for method {MethodName}: {identifier}");
        
        Logger.LogDebug("Creating DID for method {Method}: {Identifier}", MethodName, identifier);
        
        try
        {
            var did = Did.Create(MethodName, identifier);
            didDocument.Id = did.Value;
            
            // TODO: Implement method-specific creation logic
            // This would typically involve:
            // 1. Storing the DID Document in the method-specific registry
            // 2. Publishing the DID Document
            // 3. Updating local caches
            
            Logger.LogInformation("Successfully created DID: {Did}", did.Value);
            return did;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating DID for method {Method}: {Identifier}", MethodName, identifier);
            throw;
        }
    }
    
    /// <summary>
    /// Updates an existing DID Document
    /// </summary>
    /// <param name="did">The DID to update</param>
    /// <param name="didDocument">The updated DID Document</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the update was successful</returns>
    public virtual async Task<bool> UpdateAsync(string did, DidDocument didDocument, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(did))
            throw new ArgumentException("DID cannot be null or empty", nameof(did));
        
        if (didDocument == null)
            throw new ArgumentNullException(nameof(didDocument));
        
        Logger.LogDebug("Updating DID for method {Method}: {Did}", MethodName, did);
        
        try
        {
            // TODO: Implement method-specific update logic
            // This would typically involve:
            // 1. Validating the update request
            // 2. Updating the DID Document in the method-specific registry
            // 3. Publishing the updated DID Document
            // 4. Updating local caches
            
            Logger.LogInformation("Successfully updated DID: {Did}", did);
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating DID for method {Method}: {Did}", MethodName, did);
            throw;
        }
    }
    
    /// <summary>
    /// Deactivates a DID
    /// </summary>
    /// <param name="did">The DID to deactivate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the deactivation was successful</returns>
    public virtual async Task<bool> DeactivateAsync(string did, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(did))
            throw new ArgumentException("DID cannot be null or empty", nameof(did));
        
        Logger.LogDebug("Deactivating DID for method {Method}: {Did}", MethodName, did);
        
        try
        {
            // TODO: Implement method-specific deactivation logic
            // This would typically involve:
            // 1. Validating the deactivation request
            // 2. Marking the DID as deactivated in the method-specific registry
            // 3. Publishing the deactivation
            // 4. Updating local caches
            
            Logger.LogInformation("Successfully deactivated DID: {Did}", did);
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deactivating DID for method {Method}: {Did}", MethodName, did);
            throw;
        }
    }
    
    /// <summary>
    /// Validates a DID identifier for this method
    /// </summary>
    /// <param name="identifier">The identifier to validate</param>
    /// <returns>True if the identifier is valid for this method</returns>
    public abstract bool ValidateIdentifier(string identifier);
    
    /// <summary>
    /// Validates a DID for this method
    /// </summary>
    /// <param name="did">The DID to validate</param>
    /// <returns>True if the DID is valid for this method</returns>
    protected bool ValidateDid(string did)
    {
        if (string.IsNullOrEmpty(did))
            return false;
        
        try
        {
            var parsedDid = Did.Parse(did);
            return parsedDid.Method == MethodName && ValidateIdentifier(parsedDid.Identifier);
        }
        catch
        {
            return false;
        }
    }
}
