using System.Text.Json.Serialization;
using Mamey.Auth.Decentralized.Exceptions;

namespace Mamey.Auth.Decentralized.VerifiableCredentials;

/// <summary>
/// Represents a Verifiable Credential as defined by W3C Verifiable Credentials Data Model 1.1
/// </summary>
public class VerifiableCredential
{
    /// <summary>
    /// The JSON-LD context for the credential
    /// </summary>
    [JsonPropertyName("@context")]
    public List<string> Context { get; set; } = new()
    {
        "https://www.w3.org/2018/credentials/v1"
    };

    /// <summary>
    /// The type of the credential
    /// </summary>
    [JsonPropertyName("type")]
    public List<string> Type { get; set; } = new()
    {
        "VerifiableCredential"
    };

    /// <summary>
    /// The unique identifier of the credential
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// The issuer of the credential
    /// </summary>
    [JsonPropertyName("issuer")]
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// The issuance date of the credential
    /// </summary>
    [JsonPropertyName("issuanceDate")]
    public DateTime IssuanceDate { get; set; }

    /// <summary>
    /// The expiration date of the credential (optional)
    /// </summary>
    [JsonPropertyName("expirationDate")]
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// The credential subject
    /// </summary>
    [JsonPropertyName("credentialSubject")]
    public CredentialSubject CredentialSubject { get; set; } = new();

    /// <summary>
    /// The proof of the credential
    /// </summary>
    [JsonPropertyName("proof")]
    public Proof? Proof { get; set; }

    /// <summary>
    /// Additional properties not defined in the W3C specification
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; } = new();

    /// <summary>
    /// Validates the credential structure
    /// </summary>
    /// <returns>True if the credential is valid, false otherwise</returns>
    public bool IsValid()
    {
        try
        {
            // Check required fields
            if (Context == null || !Context.Any())
                return false;

            if (Type == null || !Type.Any())
                return false;

            if (string.IsNullOrEmpty(Issuer))
                return false;

            if (CredentialSubject == null)
                return false;

            // Check dates
            if (ExpirationDate.HasValue && ExpirationDate.Value <= IssuanceDate)
                return false;

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if the credential is expired
    /// </summary>
    /// <returns>True if the credential is expired, false otherwise</returns>
    public bool IsExpired()
    {
        return ExpirationDate.HasValue && ExpirationDate.Value <= DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a copy of the credential
    /// </summary>
    /// <returns>A copy of the credential</returns>
    public VerifiableCredential Clone()
    {
        return new VerifiableCredential
        {
            Context = new List<string>(Context),
            Type = new List<string>(Type),
            Id = Id,
            Issuer = Issuer,
            IssuanceDate = IssuanceDate,
            ExpirationDate = ExpirationDate,
            CredentialSubject = CredentialSubject.Clone(),
            Proof = Proof?.Clone(),
            AdditionalProperties = new Dictionary<string, JsonElement>(AdditionalProperties)
        };
    }
}
