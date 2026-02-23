using System.Text.Json.Serialization;

namespace Mamey.Identity.Decentralized.Core;

/// <summary>
/// Represents a cryptographic proof attached to a DID Document (per spec).
/// </summary>
public class Proof
{
    /// <summary>
    /// Type of the proof (e.g., "Ed25519Signature2018").
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// Purpose of the proof (e.g., "authentication").
    /// </summary>
    [JsonPropertyName("proofPurpose")]
    public string ProofPurpose { get; set; }

    /// <summary>
    /// Controller that produced the proof.
    /// </summary>
    [JsonPropertyName("verificationMethod")]
    public string VerificationMethod { get; set; }

    /// <summary>
    /// Signature value or JWS.
    /// </summary>
    [JsonPropertyName("proofValue")]
    public string ProofValue { get; set; }

    /// <summary>
    /// Timestamp of proof creation.
    /// </summary>
    [JsonPropertyName("created")]
    public DateTimeOffset? Created { get; set; }

    /// <summary>
    /// Domain or challenge, if present.
    /// </summary>
    [JsonPropertyName("Domain")]
    public string Domain { get; set; }

    public string Challenge { get; set; }

    /// <summary>
    /// Extension/custom fields.
    /// </summary>
    [JsonPropertyName("additionalProperties")]
    public IDictionary<string, object> AdditionalProperties { get; set; }
}