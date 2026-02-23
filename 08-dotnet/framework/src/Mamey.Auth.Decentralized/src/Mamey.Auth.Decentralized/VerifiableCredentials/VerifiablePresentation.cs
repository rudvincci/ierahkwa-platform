using System.Text.Json.Serialization;
using Mamey.Auth.Decentralized.Exceptions;

namespace Mamey.Auth.Decentralized.VerifiableCredentials;

/// <summary>
/// Represents a Verifiable Presentation as defined by W3C Verifiable Credentials Data Model 1.1
/// </summary>
public class VerifiablePresentation
{
    /// <summary>
    /// The JSON-LD context for the presentation
    /// </summary>
    [JsonPropertyName("@context")]
    public List<string> Context { get; set; } = new()
    {
        "https://www.w3.org/2018/credentials/v1"
    };

    /// <summary>
    /// The type of the presentation
    /// </summary>
    [JsonPropertyName("type")]
    public List<string> Type { get; set; } = new()
    {
        "VerifiablePresentation"
    };

    /// <summary>
    /// The unique identifier of the presentation
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// The holder of the presentation
    /// </summary>
    [JsonPropertyName("holder")]
    public string? Holder { get; set; }

    /// <summary>
    /// The verifiable credentials in the presentation
    /// </summary>
    [JsonPropertyName("verifiableCredential")]
    public List<VerifiableCredential> VerifiableCredentials { get; set; } = new();

    /// <summary>
    /// The proof of the presentation
    /// </summary>
    [JsonPropertyName("proof")]
    public Proof? Proof { get; set; }

    /// <summary>
    /// Additional properties not defined in the W3C specification
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; } = new();

    /// <summary>
    /// Validates the presentation structure
    /// </summary>
    /// <returns>True if the presentation is valid, false otherwise</returns>
    public bool IsValid()
    {
        try
        {
            // Check required fields
            if (Context == null || !Context.Any())
                return false;

            if (Type == null || !Type.Any())
                return false;

            // Validate all credentials
            if (VerifiableCredentials != null)
            {
                foreach (var credential in VerifiableCredentials)
                {
                    if (credential == null || !credential.IsValid())
                        return false;
                }
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if any credential in the presentation is expired
    /// </summary>
    /// <returns>True if any credential is expired, false otherwise</returns>
    public bool HasExpiredCredentials()
    {
        return VerifiableCredentials?.Any(c => c.IsExpired()) ?? false;
    }

    /// <summary>
    /// Gets all expired credentials
    /// </summary>
    /// <returns>List of expired credentials</returns>
    public List<VerifiableCredential> GetExpiredCredentials()
    {
        return VerifiableCredentials?.Where(c => c.IsExpired()).ToList() ?? new List<VerifiableCredential>();
    }

    /// <summary>
    /// Creates a copy of the presentation
    /// </summary>
    /// <returns>A copy of the presentation</returns>
    public VerifiablePresentation Clone()
    {
        return new VerifiablePresentation
        {
            Context = new List<string>(Context),
            Type = new List<string>(Type),
            Id = Id,
            Holder = Holder,
            VerifiableCredentials = VerifiableCredentials?.Select(c => c.Clone()).ToList() ?? new List<VerifiableCredential>(),
            Proof = Proof?.Clone(),
            AdditionalProperties = new Dictionary<string, JsonElement>(AdditionalProperties)
        };
    }
}
