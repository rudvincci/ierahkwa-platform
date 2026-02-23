using System.Text.Json.Serialization;
using Mamey.Auth.Decentralized.Exceptions;

namespace Mamey.Auth.Decentralized.VerifiableCredentials;

/// <summary>
/// Represents a cryptographic proof as defined by W3C Verifiable Credentials Data Model 1.1
/// </summary>
public class Proof
{
    /// <summary>
    /// The type of the proof
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// The method used to create the proof
    /// </summary>
    [JsonPropertyName("proofMethod")]
    public string? ProofMethod { get; set; }

    /// <summary>
    /// The date when the proof was created
    /// </summary>
    [JsonPropertyName("created")]
    public DateTime Created { get; set; }

    /// <summary>
    /// The domain for the proof
    /// </summary>
    [JsonPropertyName("domain")]
    public string? Domain { get; set; }

    /// <summary>
    /// The challenge for the proof
    /// </summary>
    [JsonPropertyName("challenge")]
    public string? Challenge { get; set; }

    /// <summary>
    /// The nonce for the proof
    /// </summary>
    [JsonPropertyName("nonce")]
    public string? Nonce { get; set; }

    /// <summary>
    /// The purpose of the proof
    /// </summary>
    [JsonPropertyName("proofPurpose")]
    public string? ProofPurpose { get; set; }

    /// <summary>
    /// The verification method used for the proof
    /// </summary>
    [JsonPropertyName("verificationMethod")]
    public string? VerificationMethod { get; set; }

    /// <summary>
    /// The cryptographic signature value
    /// </summary>
    [JsonPropertyName("jws")]
    public string? Jws { get; set; }

    /// <summary>
    /// The cryptographic signature value (alternative property name)
    /// </summary>
    [JsonPropertyName("signatureValue")]
    public string? SignatureValue { get; set; }

    /// <summary>
    /// Additional properties not defined in the W3C specification
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; } = new();

    /// <summary>
    /// Validates the proof structure
    /// </summary>
    /// <returns>True if the proof is valid, false otherwise</returns>
    public bool IsValid()
    {
        try
        {
            // Check required fields
            if (string.IsNullOrEmpty(Type))
                return false;

            if (string.IsNullOrEmpty(ProofMethod) && string.IsNullOrEmpty(VerificationMethod))
                return false;

            // Check that at least one signature value is present
            if (string.IsNullOrEmpty(Jws) && string.IsNullOrEmpty(SignatureValue))
                return false;

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets the signature value (JWS or SignatureValue)
    /// </summary>
    /// <returns>The signature value</returns>
    public string? GetSignatureValue()
    {
        return Jws ?? SignatureValue;
    }

    /// <summary>
    /// Sets the signature value (sets both JWS and SignatureValue for compatibility)
    /// </summary>
    /// <param name="value">The signature value</param>
    public void SetSignatureValue(string value)
    {
        Jws = value;
        SignatureValue = value;
    }

    /// <summary>
    /// Creates a copy of the proof
    /// </summary>
    /// <returns>A copy of the proof</returns>
    public Proof Clone()
    {
        return new Proof
        {
            Type = Type,
            ProofMethod = ProofMethod,
            Created = Created,
            Domain = Domain,
            Challenge = Challenge,
            Nonce = Nonce,
            ProofPurpose = ProofPurpose,
            VerificationMethod = VerificationMethod,
            Jws = Jws,
            SignatureValue = SignatureValue,
            AdditionalProperties = new Dictionary<string, JsonElement>(AdditionalProperties)
        };
    }
}
