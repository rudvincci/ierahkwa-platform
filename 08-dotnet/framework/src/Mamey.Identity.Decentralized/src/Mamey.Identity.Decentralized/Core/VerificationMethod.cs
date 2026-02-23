using Mamey.Identity.Decentralized.Abstractions;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace Mamey.Identity.Decentralized.Core;

/// <summary>
/// Implements a W3C-compliant Verification Method for use in DID Documents.
/// </summary>
public class VerificationMethod : IDidVerificationMethod
{
    /// <summary>
    /// The identifier for this verification method (usually a DID fragment or URL).
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; }

    /// <summary>
    /// The type of cryptographic key or method (e.g., "Ed25519VerificationKey2018").
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; }

    /// <summary>
    /// The controller (DID or URI) responsible for this verification method.
    /// </summary>
    [JsonPropertyName("controller")]
    public string Controller { get; }

    /// <summary>
    /// The public key material, which may be JWK, Multibase, or base58 encoded, depending on method.
    /// </summary>
    [JsonPropertyName("publicKeyJwk")]
    public IReadOnlyDictionary<string, object> PublicKeyJwk { get; }

    /// <summary>
    /// Raw public key material, base58 encoded.
    /// </summary>
    [JsonPropertyName("publicKeyBase58")]
    public string PublicKeyBase58 { get; }

    /// <summary>
    /// Raw public key material, multibase encoded.
    /// </summary>
    [JsonPropertyName("publicKeyMultibase")]
    public string PublicKeyMultibase { get; }

    /// <summary>
    /// Additional method-specific properties.
    /// </summary>
    [JsonExtensionData]
    public IReadOnlyDictionary<string, object> AdditionalProperties { get; }

    /// <summary>
    /// Constructs a VerificationMethod object.
    /// </summary>
    /// <param name="id">The unique identifier for the verification method.</param>
    /// <param name="type">The type of key or verification method.</param>
    /// <param name="controller">The controller for this verification method.</param>
    /// <param name="publicKeyJwk">JWK dictionary, if present.</param>
    /// <param name="publicKeyBase58">Base58-encoded key string.</param>
    /// <param name="publicKeyMultibase">Multibase-encoded key string.</param>
    /// <param name="additionalProperties">Any additional properties.</param>
    public VerificationMethod(
        string id,
        string type,
        string controller,
        IDictionary<string, object> publicKeyJwk = null,
        string publicKeyBase58 = null,
        string publicKeyMultibase = null,
        IDictionary<string, object> additionalProperties = null)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Type = type;
        Controller = controller;
        PublicKeyJwk = publicKeyJwk != null
            ? new Dictionary<string, object>(publicKeyJwk)
            : new Dictionary<string, object>();
        PublicKeyBase58 = publicKeyBase58;
        PublicKeyMultibase = publicKeyMultibase;
        AdditionalProperties = additionalProperties != null
            ? new Dictionary<string, object>(additionalProperties)
            : new Dictionary<string, object>();
    }

    /// <summary>
    /// Extracts the public key bytes from the preferred encoding (multibase, base58, or jwk).
    /// Throws if none are present.
    /// </summary>
    /// <returns>The decoded public key bytes.</returns>
    public byte[] GetPublicKeyBytes()
    {
        if (!string.IsNullOrWhiteSpace(PublicKeyMultibase))
            return Crypto.MultibaseUtil.Decode(PublicKeyMultibase);
        if (!string.IsNullOrWhiteSpace(PublicKeyBase58))
            return Crypto.Base58Util.Decode(PublicKeyBase58);
        if (PublicKeyJwk != null && PublicKeyJwk.Count > 0)
        {
            // Ed25519/secp256k1 (OKP/EC): "x" (base64url)
            if (PublicKeyJwk.TryGetValue("x", out var xval) && xval is string xstr)
                return Crypto.Base64Url.Decode(xstr);
            // RSA: "n"/"e" (not implemented here)
            throw new NotSupportedException("JWK format present but not supported by current implementation.");
        }
        throw new InvalidOperationException("No usable public key found in VerificationMethod.");
    }

    /// <summary>
    /// Parses a JObject into a VerificationMethod instance.
    /// </summary>
    /// <param name="obj">The JObject representing the verification method.</param>
    /// <returns>The parsed VerificationMethod instance.</returns>
    public static VerificationMethod Parse(JObject obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj), "VerificationMethod JSON cannot be null.");

        string id = obj["id"]?.Value<string>()
                    ?? throw new FormatException("VerificationMethod missing required 'id'.");
        string type = obj["type"]?.Value<string>()
                      ?? throw new FormatException("VerificationMethod missing required 'type'.");
        string controller = obj["controller"]?.Value<string>() ?? string.Empty;

        IReadOnlyDictionary<string, object> publicKeyJwk = null;
        var jwkToken = obj["publicKeyJwk"];
        if (jwkToken is JObject jwkObj)
            publicKeyJwk = jwkObj.Properties().ToDictionary(p => p.Name, p => (object)p.Value);

        string publicKeyBase58 = obj["publicKeyBase58"]?.Value<string>();
        string publicKeyMultibase = obj["publicKeyMultibase"]?.Value<string>();

        var knownProps = new[]
            { "id", "type", "controller", "publicKeyJwk", "publicKeyBase58", "publicKeyMultibase" };
        var additional = obj.Properties()
            .Where(p => !knownProps.Contains(p.Name))
            .ToDictionary(p => p.Name, p => (object)p.Value);

        return new VerificationMethod(
            id,
            type,
            controller,
            (IDictionary<string, object>)publicKeyJwk,
            publicKeyBase58,
            publicKeyMultibase,
            additional
        );
    }

    /// <summary>
    /// Converts this VerificationMethod to a Newtonsoft JObject.
    /// </summary>
    public JObject ToJsonObject()
    {
        var obj = new JObject
        {
            ["id"] = Id,
            ["type"] = Type,
            ["controller"] = Controller
        };

        if (PublicKeyJwk != null && PublicKeyJwk.Count > 0)
            obj["publicKeyJwk"] = JObject.FromObject(PublicKeyJwk);
        if (!string.IsNullOrWhiteSpace(PublicKeyBase58))
            obj["publicKeyBase58"] = PublicKeyBase58;
        if (!string.IsNullOrWhiteSpace(PublicKeyMultibase))
            obj["publicKeyMultibase"] = PublicKeyMultibase;
        if (AdditionalProperties != null)
        {
            foreach (var prop in AdditionalProperties)
            {
                if (!obj.ContainsKey(prop.Key))
                    obj[prop.Key] = prop.Value is JToken jt ? jt : JToken.FromObject(prop.Value);
            }
        }
        return obj;
    }

    /// <summary>
    /// Serializes this VerificationMethod to a JSON string.
    /// </summary>
    /// <returns>JSON string representation.</returns>
    public string ToJson()
    {
        return ToJsonObject().ToString();
    }

    /// <summary>
    /// Parses a JSON string into a VerificationMethod instance.
    /// </summary>
    /// <param name="json">The JSON string to parse.</param>
    /// <returns>The parsed VerificationMethod.</returns>
    public static VerificationMethod Parse(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentNullException(nameof(json), "VerificationMethod JSON cannot be null or empty.");
        JObject obj;
        try { obj = JObject.Parse(json); }
        catch (Exception ex)
        {
            throw new FormatException("Invalid JSON provided to VerificationMethod.Parse(string).", ex);
        }
        return Parse(obj);
    }
}