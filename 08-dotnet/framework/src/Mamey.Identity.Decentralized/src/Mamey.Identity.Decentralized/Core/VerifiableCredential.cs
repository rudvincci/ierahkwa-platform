using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Mamey.Identity.Decentralized.Converters;

namespace Mamey.Identity.Decentralized.Core;

/// <summary>
/// W3C Verifiable Credential (VC) data model, as per https://www.w3.org/TR/vc-data-model/.
/// Supports all standard and extension fields, including JSON-LD compatibility.
/// </summary>
public class VerifiableCredential
{
    /// <summary>
    /// REQUIRED. The @context property expresses the JSON-LD context(s) for the VC.
    /// Most VCs will use at least "https://www.w3.org/2018/credentials/v1".
    /// </summary>
    [JsonPropertyName("@context")]
    public List<object> Context { get; set; } = new() { "https://www.w3.org/2018/credentials/v1" };

    /// <summary>
    /// OPTIONAL. The identifier of the credential (may be a URI, blank node, or omitted).
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// REQUIRED. One or more types describing the credential (e.g. ["VerifiableCredential"]).
    /// </summary>
    [JsonPropertyName("type")]
    public List<string> Type { get; set; } = new() { "VerifiableCredential" };

    /// <summary>
    /// REQUIRED. The credential issuer, typically a DID or URI, or an object with an id field.
    /// </summary>
    [JsonPropertyName("issuer")]
    [JsonConverter(typeof(IssuerConverter))]
    public object Issuer { get; set; }

    /// <summary>
    /// REQUIRED. The issuance date of the credential (ISO 8601 UTC).
    /// </summary>
    [JsonPropertyName("issuanceDate")]
    public DateTime IssuanceDate { get; set; }

    /// <summary>
    /// OPTIONAL. The expiration date of the credential, if applicable.
    /// </summary>
    [JsonPropertyName("expirationDate")]
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// REQUIRED. The credential subject(s), either a single object or array.
    /// </summary>
    [JsonPropertyName("credentialSubject")]
    [JsonConverter(typeof(CredentialSubjectConverter))]
    public object CredentialSubject { get; set; }

    /// <summary>
    /// OPTIONAL. Credential status (e.g. revocation, suspension), per W3C status list spec.
    /// </summary>
    [JsonPropertyName("credentialStatus")]
    public CredentialStatus CredentialStatus { get; set; }

    /// <summary>
    /// OPTIONAL. Credential schema references for validation.
    /// </summary>
    [JsonPropertyName("credentialSchema")]
    public List<CredentialSchema> CredentialSchema { get; set; }

    /// <summary>
    /// OPTIONAL. Evidence or attestation references.
    /// </summary>
    [JsonPropertyName("evidence")]
    public List<object> Evidence { get; set; }

    /// <summary>
    /// OPTIONAL. Terms of use or policy references.
    /// </summary>
    [JsonPropertyName("termsOfUse")]
    public List<object> TermsOfUse { get; set; }

    /// <summary>
    /// OPTIONAL. Linked Data Proof or JWT (as object or string).
    /// </summary>
    [JsonPropertyName("proof")]
    public object Proof { get; set; }

    /// <summary>
    /// For additional arbitrary extension properties.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object> AdditionalProperties { get; set; } = new();
    
    // public int CredentialSubjectCount => CredentialSubjectList.Count();
    // public IEnumerable<object> CredentialSubjectList => CredentialSubject switch
    // {
    //     List<object> list => list,
    //     List<CredentialSubject> list => list,
    //     Dictionary<string, object> dict => new[] { dict },
    //     string s when !string.IsNullOrEmpty(s) => new[] { s },
    //     _ => Array.Empty<object>()
    // };
    

    /// <summary>
    /// Parse a Verifiable Credential from a JSON string.
    /// Handles both single and array forms for credentialSubject, issuer, etc.
    /// Throws with detailed error message if invalid or non-conforming.
    /// </summary>
    /// 
    public static VerifiableCredential Parse(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("VerifiableCredential JSON input is empty or null.", nameof(json));

        JsonNode node;
        try
        {
            node = JsonNode.Parse(json, documentOptions: new JsonDocumentOptions { AllowTrailingCommas = true });
        }
        catch (Exception ex)
        {
            throw new FormatException("VC JSON parsing failed: " + ex.Message, ex);
        }

        if (node is not JsonObject obj)
            throw new FormatException("VerifiableCredential JSON root must be an object.");

        // Context
        if (!obj.TryGetPropertyValue("@context", out var contextNode) || contextNode is null)
            throw new FormatException("Missing @context in VerifiableCredential.");
        var contextList = contextNode is JsonArray arr
            ? arr.Select(x => x.GetValue<object>()).ToList()
            : new List<object> { contextNode.GetValue<object>() };
        if (contextList.Count == 0)
            throw new FormatException("@context must not be empty in VerifiableCredential.");

        // Type
        if (!obj.TryGetPropertyValue("type", out var typeNode) || typeNode is null)
            throw new FormatException("Missing type in VerifiableCredential.");
        var typeList = typeNode is JsonArray tarr
            ? tarr.Select(x => x.GetValue<string>()).ToList()
            : new List<string> { typeNode.GetValue<string>() };
        if (!typeList.Any())
            throw new FormatException("type must not be empty in VerifiableCredential.");

        // Issuer
        if (!obj.TryGetPropertyValue("issuer", out var issuerNode) || issuerNode is null)
            throw new FormatException("Missing issuer in VerifiableCredential.");
        object issuer;
        if (issuerNode is JsonObject issuerObj && issuerObj.TryGetPropertyValue("id", out var idNode) &&
            idNode is not null)
            issuer = idNode.GetValue<string>();
        else if (issuerNode is JsonValue)
            issuer = issuerNode.GetValue<string>();
        else
            throw new FormatException("issuer must be a string or an object with 'id'.");

        // IssuanceDate
        if (!obj.TryGetPropertyValue("issuanceDate", out var issuanceDateNode) || issuanceDateNode is null)
            throw new FormatException("Missing issuanceDate in VerifiableCredential.");
        if (!DateTime.TryParse(issuanceDateNode.ToString(), null, System.Globalization.DateTimeStyles.AdjustToUniversal,
                out var issuanceDate))
            throw new FormatException("issuanceDate is not a valid ISO 8601 UTC date.");

        // CredentialSubject (object or array, normalize to List<CredentialSubject>)
        if (!obj.TryGetPropertyValue("credentialSubject", out var subjectNode) || subjectNode is null)
            throw new FormatException("Missing credentialSubject in VerifiableCredential.");
        List<CredentialSubject> subjects = new();
        if (subjectNode is JsonArray subjArr)
        {
            foreach (var s in subjArr)
            {
                var subj = s.Deserialize<CredentialSubject>(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (subj == null)
                    throw new FormatException("credentialSubject element is invalid.");
                subjects.Add(subj);
            }
        }
        else if (subjectNode is JsonObject || subjectNode is JsonValue)
        {
            var subj = subjectNode.Deserialize<CredentialSubject>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (subj == null)
                throw new FormatException("credentialSubject is invalid.");
            subjects.Add(subj);
        }
        else
        {
            throw new FormatException("credentialSubject must be an object or array of objects.");
        }

        // Optional: proof
        object? proof = null;
        if (obj.TryGetPropertyValue("proof", out var proofNode) && proofNode is not null)
            proof = proofNode.Deserialize<object>();

        // Optional: credentialStatus
        CredentialStatus? credentialStatus = null;
        if (obj.TryGetPropertyValue("credentialStatus", out var statusNode) && statusNode is not null)
        {
            try
            {
                credentialStatus = statusNode.Deserialize<CredentialStatus>();
            }
            catch (Exception ex)
            {
                throw new FormatException("credentialStatus parsing failed: " + ex.Message, ex);
            }
        }

        // Optional: expirationDate
        DateTime? expirationDate = null;
        if (obj.TryGetPropertyValue("expirationDate", out var expNode) && expNode is not null)
        {
            if (DateTime.TryParse(expNode.ToString(), null, System.Globalization.DateTimeStyles.AdjustToUniversal,
                    out var dt))
                expirationDate = dt;
        }

        // Optional: credentialSchema
        List<CredentialSchema>? credentialSchemas = null;
        if (obj.TryGetPropertyValue("credentialSchema", out var schemaNode) && schemaNode is not null)
        {
            try
            {
                credentialSchemas = schemaNode.Deserialize<List<CredentialSchema>>();
            }
            catch
            {
            }
        }

        // Optional: evidence
        List<object>? evidence = null;
        if (obj.TryGetPropertyValue("evidence", out var evidenceNode) && evidenceNode is not null)
        {
            try
            {
                evidence = evidenceNode is JsonArray arrE
                    ? arrE.Select(x => x.Deserialize<object>()).ToList()
                    : new List<object> { evidenceNode.Deserialize<object>() };
            }
            catch
            {
            }
        }

        // Optional: termsOfUse
        List<object>? termsOfUse = null;
        if (obj.TryGetPropertyValue("termsOfUse", out var termsNode) && termsNode is not null)
        {
            try
            {
                termsOfUse = termsNode is JsonArray arrT
                    ? arrT.Select(x => x.Deserialize<object>()).ToList()
                    : new List<object> { termsNode.Deserialize<object>() };
            }
            catch
            {
            }
        }

        // Additional extension properties
        var additionalProps = obj.Where(kvp =>
            kvp.Key != "@context" && kvp.Key != "id" && kvp.Key != "type" && kvp.Key != "issuer" &&
            kvp.Key != "issuanceDate" && kvp.Key != "expirationDate" && kvp.Key != "credentialSubject" &&
            kvp.Key != "credentialStatus" && kvp.Key != "credentialSchema" && kvp.Key != "evidence" &&
            kvp.Key != "termsOfUse" && kvp.Key != "proof"
        ).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Deserialize<object>());

        // Construct VC
        string? id = null;
        JsonNode? idNodeFromJsonNode = null;
        if (obj.TryGetPropertyValue("id", out idNodeFromJsonNode) && idNodeFromJsonNode is not null)
            id = idNodeFromJsonNode.GetValue<string>();
        var vc = new VerifiableCredential
        {
            Context = contextList,
            Id = id,
            Type = typeList,
            Issuer = issuer,
            IssuanceDate = issuanceDate,
            ExpirationDate = expirationDate,
            CredentialSubject = subjects, // always a list
            CredentialStatus = credentialStatus,
            CredentialSchema = credentialSchemas,
            Evidence = evidence,
            TermsOfUse = termsOfUse,
            Proof = proof,
            AdditionalProperties = additionalProps
        };
        return vc;
    }
    public string ToJson(bool indented = true)
    {
        var opts = new JsonSerializerOptions
        {
            WriteIndented = indented,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        // Copy this object to a dynamic structure, but normalize CredentialSubject per spec:
        var clone = new Dictionary<string, object?>();

        // Copy standard properties
        clone["@context"] = Context;
        if (Id != null) clone["id"] = Id;
        clone["type"] = Type;
        clone["issuer"] = Issuer;
        clone["issuanceDate"] = IssuanceDate.ToString("o");
        if (ExpirationDate != null) clone["expirationDate"] = ExpirationDate.Value.ToString("o");
        if (CredentialStatus != null) clone["credentialStatus"] = CredentialStatus;
        if (CredentialSchema != null) clone["credentialSchema"] = CredentialSchema;
        if (Evidence != null) clone["evidence"] = Evidence;
        if (TermsOfUse != null) clone["termsOfUse"] = TermsOfUse;
        if (Proof != null) clone["proof"] = Proof;

        // CredentialSubject: single object or array
        if (((IEnumerable<CredentialSubject>)CredentialSubject).Count() == 1)
            clone["credentialSubject"] = ((IEnumerable<CredentialSubject>)CredentialSubject).ElementAt(0);
        else if (CredentialSubject != null && ((IEnumerable<CredentialSubject>)CredentialSubject).Count() > 1)
            clone["credentialSubject"] = CredentialSubject;
        // else (should not happen with valid VC)

        // Add extension properties
        if (AdditionalProperties != null)
        {
            foreach (var kvp in AdditionalProperties)
                clone[kvp.Key] = kvp.Value;
        }

        // Serialize
        return JsonSerializer.Serialize(clone, opts);
    }

}