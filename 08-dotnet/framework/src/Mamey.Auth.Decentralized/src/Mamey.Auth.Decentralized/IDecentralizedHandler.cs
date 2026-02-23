using Mamey.Auth.Decentralized.Core;
using Mamey.Auth.Decentralized.Crypto;

namespace Mamey.Auth.Decentralized;

/// <summary>
/// Main interface for decentralized authentication operations
/// </summary>
public interface IDecentralizedHandler
{
    /// <summary>
    /// Resolves a DID to its DID Document
    /// </summary>
    /// <param name="did">The DID to resolve</param>
    /// <param name="options">Optional resolution options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The resolution result</returns>
    Task<DidResolutionResult> ResolveDidAsync(string did, DidResolutionOptions? options = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Dereferences a DID URL to its content
    /// </summary>
    /// <param name="didUrl">The DID URL to dereference</param>
    /// <param name="options">Optional dereferencing options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The dereferencing result</returns>
    Task<DidDereferencingResult> DereferenceDidUrlAsync(string didUrl, DidDereferencingOptions? options = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new DID with the specified method
    /// </summary>
    /// <param name="method">The DID method</param>
    /// <param name="identifier">The method-specific identifier</param>
    /// <param name="didDocument">The DID Document to associate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created DID</returns>
    Task<Did> CreateDidAsync(string method, string identifier, DidDocument didDocument, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing DID Document
    /// </summary>
    /// <param name="did">The DID to update</param>
    /// <param name="didDocument">The updated DID Document</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the update was successful</returns>
    Task<bool> UpdateDidDocumentAsync(string did, DidDocument didDocument, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deactivates a DID
    /// </summary>
    /// <param name="did">The DID to deactivate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the deactivation was successful</returns>
    Task<bool> DeactivateDidAsync(string did, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generates a new key pair for the specified algorithm
    /// </summary>
    /// <param name="algorithm">The cryptographic algorithm</param>
    /// <param name="curveName">Optional curve name for elliptic curve algorithms</param>
    /// <returns>A new key pair</returns>
    KeyPair GenerateKeyPair(string algorithm, string? curveName = null);
    
    /// <summary>
    /// Generates a new key pair with a seed
    /// </summary>
    /// <param name="algorithm">The cryptographic algorithm</param>
    /// <param name="seed">The seed for key generation</param>
    /// <param name="curveName">Optional curve name for elliptic curve algorithms</param>
    /// <returns>A new key pair</returns>
    KeyPair GenerateKeyPair(string algorithm, byte[] seed, string? curveName = null);
    
    /// <summary>
    /// Signs data with the specified key pair
    /// </summary>
    /// <param name="data">The data to sign</param>
    /// <param name="keyPair">The key pair to use for signing</param>
    /// <returns>The signature</returns>
    byte[] SignData(byte[] data, KeyPair keyPair);
    
    /// <summary>
    /// Verifies a signature with the specified public key
    /// </summary>
    /// <param name="data">The original data</param>
    /// <param name="signature">The signature to verify</param>
    /// <param name="publicKey">The public key</param>
    /// <param name="algorithm">The algorithm used for the signature</param>
    /// <returns>True if the signature is valid</returns>
    bool VerifySignature(byte[] data, byte[] signature, byte[] publicKey, string algorithm);
    
    /// <summary>
    /// Validates a DID Document according to W3C DID 1.1 specification
    /// </summary>
    /// <param name="didDocument">The DID Document to validate</param>
    /// <returns>True if the document is valid</returns>
    bool ValidateDidDocument(DidDocument didDocument);
    
    /// <summary>
    /// Validates a DID according to W3C DID 1.1 specification
    /// </summary>
    /// <param name="did">The DID to validate</param>
    /// <returns>True if the DID is valid</returns>
    bool ValidateDid(string did);
    
    /// <summary>
    /// Gets the supported DID methods
    /// </summary>
    /// <returns>List of supported DID methods</returns>
    IEnumerable<string> GetSupportedDidMethods();
    
    /// <summary>
    /// Gets the supported cryptographic algorithms
    /// </summary>
    /// <returns>List of supported algorithms</returns>
    IEnumerable<string> GetSupportedAlgorithms();
}
