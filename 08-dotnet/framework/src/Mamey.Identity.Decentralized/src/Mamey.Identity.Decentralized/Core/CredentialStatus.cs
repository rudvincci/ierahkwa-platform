using System.Text.Json.Serialization;

namespace Mamey.Identity.Decentralized.Core;

/// <summary>
/// W3C Credential Status object.
/// See: https://www.w3.org/TR/vc-data-model/#status
/// Supports "StatusList2021Entry" and extensible types.
/// </summary>
public class CredentialStatus
{
    /// <summary>
    /// REQUIRED. The unique URI for this status entry.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// REQUIRED. The type of status (e.g. "StatusList2021Entry").
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// OPTIONAL. The status purpose (e.g. "revocation", "suspension").
    /// </summary>
    [JsonPropertyName("statusPurpose")]
    public string StatusPurpose { get; set; }

    /// <summary>
    /// OPTIONAL. Index in the status list bitstring.
    /// </summary>
    [JsonPropertyName("statusListIndex")]
    public string StatusListIndex { get; set; }

    /// <summary>
    /// OPTIONAL. The credential (list) where the status is asserted.
    /// </summary>
    [JsonPropertyName("statusListCredential")]
    public string StatusListCredential { get; set; }
}