using Mamey.Auth.Decentralized.Core;
using Mamey.Auth.Decentralized.Methods;
using Mamey.Auth.Decentralized.Exceptions;
using Microsoft.Extensions.Logging;

namespace Mamey.Auth.Decentralized.Resolution;

/// <summary>
/// Universal DID resolver that supports multiple DID methods
/// </summary>
public class DidResolver : IDidResolver
{
    private readonly IDidMethodRegistry _methodRegistry;
    private readonly ILogger<DidResolver> _logger;
    
    /// <summary>
    /// Initializes a new instance of the DidResolver class
    /// </summary>
    /// <param name="methodRegistry">The DID method registry</param>
    /// <param name="logger">The logger</param>
    public DidResolver(IDidMethodRegistry methodRegistry, ILogger<DidResolver> logger)
    {
        _methodRegistry = methodRegistry ?? throw new ArgumentNullException(nameof(methodRegistry));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <summary>
    /// Resolves a DID to its DID Document
    /// </summary>
    /// <param name="did">The DID to resolve</param>
    /// <param name="options">Optional resolution options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The resolution result</returns>
    public async Task<DidResolutionResult> ResolveAsync(string did, DidResolutionOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(did))
            throw new ArgumentException("DID cannot be null or empty", nameof(did));
        
        _logger.LogDebug("Resolving DID: {Did}", did);
        
        try
        {
            // Parse the DID to extract the method
            var parsedDid = Did.Parse(did);
            
            // Get the method resolver
            var method = _methodRegistry.GetMethod(parsedDid.Method);
            if (method == null)
            {
                _logger.LogWarning("Unsupported DID method: {Method}", parsedDid.Method);
                return DidResolutionResult.MethodNotSupported(did);
            }
            
            // Resolve using the method-specific resolver
            var result = await method.ResolveAsync(did, options, cancellationToken);
            
            _logger.LogDebug("DID resolution completed: {Did}, Success: {Success}", did, result.IsSuccessful);
            return result;
        }
        catch (InvalidDidException ex)
        {
            _logger.LogWarning("Invalid DID format: {Did}, Error: {Error}", did, ex.Message);
            return DidResolutionResult.InvalidDid(did);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving DID: {Did}", did);
            return DidResolutionResult.Failed(did, "resolutionError", ex.Message);
        }
    }
    
    /// <summary>
    /// Dereferences a DID URL to its content
    /// </summary>
    /// <param name="didUrl">The DID URL to dereference</param>
    /// <param name="options">Optional dereferencing options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The dereferencing result</returns>
    public async Task<DidDereferencingResult> DereferenceAsync(string didUrl, DidDereferencingOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(didUrl))
            throw new ArgumentException("DID URL cannot be null or empty", nameof(didUrl));
        
        _logger.LogDebug("Dereferencing DID URL: {DidUrl}", didUrl);
        
        try
        {
            // Parse the DID URL to extract the method
            var parsedDidUrl = DidUrl.Parse(didUrl);
            
            // Get the method resolver
            var method = _methodRegistry.GetMethod(parsedDidUrl.Did.Method);
            if (method == null)
            {
                _logger.LogWarning("Unsupported DID method: {Method}", parsedDidUrl.Did.Method);
                return DidDereferencingResult.Failed("methodNotSupported", $"DID method '{parsedDidUrl.Did.Method}' is not supported");
            }
            
            // Dereference using the method-specific resolver
            var result = await method.DereferenceAsync(didUrl, options, cancellationToken);
            
            _logger.LogDebug("DID URL dereferencing completed: {DidUrl}, Success: {Success}", didUrl, result.IsSuccessful);
            return result;
        }
        catch (InvalidDidException ex)
        {
            _logger.LogWarning("Invalid DID URL format: {DidUrl}, Error: {Error}", didUrl, ex.Message);
            return DidDereferencingResult.Failed("invalidDidUrl", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error dereferencing DID URL: {DidUrl}", didUrl);
            return DidDereferencingResult.Failed("dereferencingError", ex.Message);
        }
    }
    
    /// <summary>
    /// Gets the supported DID methods
    /// </summary>
    /// <returns>List of supported DID methods</returns>
    public IEnumerable<string> GetSupportedMethods()
    {
        return _methodRegistry.GetMethodNames();
    }
    
    /// <summary>
    /// Checks if a DID method is supported
    /// </summary>
    /// <param name="method">The DID method to check</param>
    /// <returns>True if the method is supported</returns>
    public bool IsMethodSupported(string method)
    {
        if (string.IsNullOrEmpty(method))
            return false;
        
        return _methodRegistry.IsMethodRegistered(method);
    }
}
