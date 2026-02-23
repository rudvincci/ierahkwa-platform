using Mamey.Auth.Decentralized.Core;
using Mamey.Auth.Decentralized.Exceptions;
using Mamey.Auth.Decentralized.Crypto;
using Microsoft.Extensions.Logging;

namespace Mamey.Auth.Decentralized.Methods.DidKey;

/// <summary>
/// DID Key method implementation
/// </summary>
public class DidKeyMethod : MethodBase
{
    private readonly DidKeyOptions _options;
    private readonly IKeyGenerator _keyGenerator;
    
    /// <summary>
    /// Gets the name of the DID method
    /// </summary>
    public override string MethodName => "key";
    
    /// <summary>
    /// Initializes a new instance of the DidKeyMethod class
    /// </summary>
    /// <param name="options">The DID Key options</param>
    /// <param name="keyGenerator">The key generator</param>
    /// <param name="logger">The logger</param>
    public DidKeyMethod(DidKeyOptions options, IKeyGenerator keyGenerator, ILogger<DidKeyMethod> logger) : base(logger)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _keyGenerator = keyGenerator ?? throw new ArgumentNullException(nameof(keyGenerator));
    }
    
    /// <summary>
    /// Resolves a DID to its DID Document
    /// </summary>
    /// <param name="did">The DID to resolve</param>
    /// <param name="options">Optional resolution options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The resolution result</returns>
    public override async Task<DidResolutionResult> ResolveAsync(string did, DidResolutionOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(did))
            throw new ArgumentException("DID cannot be null or empty", nameof(did));
        
        Logger.LogDebug("Resolving DID Key: {Did}", did);
        
        try
        {
            var parsedDid = Did.Parse(did);
            if (parsedDid.Method != MethodName)
            {
                return DidResolutionResult.MethodNotSupported(did);
            }
            
            if (!ValidateIdentifier(parsedDid.Identifier))
            {
                return DidResolutionResult.InvalidDid(did);
            }
            
            // Generate the DID Document from the key
            var didDocument = await DidKeyGenerator.GenerateDidDocumentAsync(parsedDid.Identifier, _keyGenerator, _options);
            
            if (didDocument == null)
            {
                return DidResolutionResult.Failed(did, "generationError", "Failed to generate DID Document from key");
            }
            
            // Validate the DID Document
            if (!didDocument.ValidateW3cCompliance())
            {
                return DidResolutionResult.Failed(did, "validationError", "Generated DID Document does not comply with W3C DID 1.1 specification");
            }
            
            var result = DidResolutionResult.Success(did, didDocument);
            result.ResolutionMetadata.Method = MethodName;
            result.ResolutionMetadata.ContentType = "application/did+json";
            
            Logger.LogDebug("Successfully resolved DID Key: {Did}", did);
            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error resolving DID Key: {Did}", did);
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
    public override async Task<DidDereferencingResult> DereferenceAsync(string didUrl, DidDereferencingOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(didUrl))
            throw new ArgumentException("DID URL cannot be null or empty", nameof(didUrl));
        
        Logger.LogDebug("Dereferencing DID Key URL: {DidUrl}", didUrl);
        
        try
        {
            var parsedDidUrl = DidUrl.Parse(didUrl);
            if (parsedDidUrl.Did.Method != MethodName)
            {
                return DidDereferencingResult.Failed("methodNotSupported", $"DID method '{parsedDidUrl.Did.Method}' is not supported");
            }
            
            if (!ValidateIdentifier(parsedDidUrl.Did.Identifier))
            {
                return DidDereferencingResult.Failed("invalidDid", "Invalid DID identifier");
            }
            
            // Generate the DID Document from the key
            var didDocument = await DidKeyGenerator.GenerateDidDocumentAsync(parsedDidUrl.Did.Identifier, _keyGenerator, _options);
            
            if (didDocument == null)
            {
                return DidDereferencingResult.Failed("generationError", "Failed to generate DID Document from key");
            }
            
            // Handle different dereferencing targets
            var content = new DidDereferencingContent();
            
            if (string.IsNullOrEmpty(parsedDidUrl.Path))
            {
                // Return the full DID Document
                content.DidDocument = didDocument;
            }
            else
            {
                // Handle specific path dereferencing
                switch (parsedDidUrl.Path.ToLowerInvariant())
                {
                    case "verificationmethod":
                        // Return verification methods
                        if (didDocument.VerificationMethod.Any())
                        {
                            content.VerificationMethod = didDocument.VerificationMethod.First();
                        }
                        break;
                    
                    case "service":
                        // Return service endpoints
                        if (didDocument.Service.Any())
                        {
                            content.Service = didDocument.Service.First();
                        }
                        break;
                    
                    default:
                        return DidDereferencingResult.Failed("notFound", $"Path '{parsedDidUrl.Path}' not found");
                }
            }
            
            var result = DidDereferencingResult.Success(content);
            result.DereferencingMetadata.ContentType = "application/did+json";
            result.DereferencingMetadata.Method = MethodName;
            
            Logger.LogDebug("Successfully dereferenced DID Key URL: {DidUrl}", didUrl);
            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error dereferencing DID Key URL: {DidUrl}", didUrl);
            return DidDereferencingResult.Failed("dereferencingError", ex.Message);
        }
    }
    
    /// <summary>
    /// Validates a DID identifier for this method
    /// </summary>
    /// <param name="identifier">The identifier to validate</param>
    /// <returns>True if the identifier is valid for this method</returns>
    public override bool ValidateIdentifier(string identifier)
    {
        if (string.IsNullOrEmpty(identifier))
            return false;
        
        try
        {
            // DID Key identifiers should be valid multibase-encoded keys
            return DidKeyGenerator.IsValidKeyIdentifier(identifier);
        }
        catch
        {
            return false;
        }
    }
}
