using System.Collections.Generic;
using System.Threading.Tasks;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Microsoft.IdentityModel.Tokens;

namespace Mamey.Auth.DecentralizedIdentifiers.Handlers;

/// <summary>
/// Interface for DID authentication handler
/// </summary>
public interface IDidHandler
{
    /// <summary>
    /// Create DID-based token
    /// </summary>
    /// <param name="did">The DID to create token for</param>
    /// <param name="claims">Additional claims</param>
    /// <returns>DID token</returns>
    Task<DidToken> CreateDidToken(string did, IDictionary<string, string> claims = null);
    
    /// <summary>
    /// Create Verifiable Presentation
    /// </summary>
    /// <param name="did">Holder DID</param>
    /// <param name="credentials">Credentials to include</param>
    /// <param name="options">Presentation options</param>
    /// <returns>Verifiable Presentation</returns>
    Task<VerifiablePresentation> CreateVerifiablePresentation(
        string did, 
        IEnumerable<VerifiableCredential> credentials,
        PresentationOptions options = null);
    
    /// <summary>
    /// Validate DID token
    /// </summary>
    /// <param name="token">Token to validate</param>
    /// <returns>True if valid</returns>
    Task<bool> ValidateDidToken(string token);
    
    /// <summary>
    /// Validate Verifiable Presentation
    /// </summary>
    /// <param name="vp">Presentation to validate</param>
    /// <returns>Validation result</returns>
    Task<PresentationValidationResult> ValidatePresentation(VerifiablePresentation vp);
    
    /// <summary>
    /// Extract payload from token
    /// </summary>
    /// <param name="token">Token to extract from</param>
    /// <returns>Token payload</returns>
    Task<DidTokenPayload> GetDidPayload(string token);
    
    /// <summary>
    /// Verify signature with DID document
    /// </summary>
    /// <param name="didDocument">DID document</param>
    /// <param name="challenge">Challenge to verify</param>
    /// <param name="signature">Signature to verify</param>
    /// <returns>True if signature is valid</returns>
    Task<bool> VerifySignatureAsync(DidDocument didDocument, string challenge, string signature);

    /// <summary>
    /// Resolve signing key by key ID
    /// </summary>
    /// <param name="keyId">Key identifier</param>
    /// <returns>Signing key</returns>
    IEnumerable<SecurityKey> ResolveSigningKey(string keyId);
    
    /// <summary>
    /// Sign data with private key
    /// </summary>
    /// <param name="data">Data to sign</param>
    /// <param name="privateKey">Private key</param>
    /// <returns>Signature</returns>
    Task<string> SignAsync(string data, string privateKey);
}

/// <summary>
/// DID token structure
/// </summary>
public class DidToken
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public long Expires { get; set; }
    public string Id { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public IDictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();
}

/// <summary>
/// DID token payload
/// </summary>
public class DidTokenPayload
{
    public string Subject { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public long Expires { get; set; }
    public IDictionary<string, IEnumerable<string>> Claims { get; set; } = new Dictionary<string, IEnumerable<string>>();
}

/// <summary>
/// Presentation options
/// </summary>
public class PresentationOptions
{
    public string Challenge { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string ProofPurpose { get; set; } = "authentication";
    public DateTime? ExpirationDate { get; set; }
}

/// <summary>
/// Presentation validation result
/// </summary>
public class PresentationValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public List<string> Warnings { get; set; } = new List<string>();
}