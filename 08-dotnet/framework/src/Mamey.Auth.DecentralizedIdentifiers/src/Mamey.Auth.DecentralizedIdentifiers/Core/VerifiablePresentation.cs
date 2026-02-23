using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mamey.Auth.DecentralizedIdentifiers.Core
{
    /// <summary>
    /// W3C Verifiable Presentation (VP) data model.
    /// See: https://www.w3.org/TR/vc-data-model/#presentations
    /// </summary>
    public class VerifiablePresentation
    {
        /// <summary>
        /// REQUIRED. JSON-LD context(s) for the presentation.
        /// Most use "https://www.w3.org/2018/credentials/v1" and may extend for ZKP, OIDC, etc.
        /// </summary>
        [JsonPropertyName("@context")]
        public List<object> Context { get; set; } = new() { "https://www.w3.org/2018/credentials/v1" };

        /// <summary>
        /// OPTIONAL. The identifier of the presentation.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// REQUIRED. One or more types for the presentation. Always includes "VerifiablePresentation".
        /// </summary>
        [JsonPropertyName("type")]
        public List<string> Type { get; set; } = new() { "VerifiablePresentation" };

        /// <summary>
        /// REQUIRED. The verifiable credentials presented. Each can be a VC object, JWT, or URI.
        /// Supports single object or array (see converter).
        /// </summary>
        [JsonPropertyName("verifiableCredential")]
        [JsonConverter(typeof(VerifiableCredentialListOrObjectConverter))]
        public object VerifiableCredential { get; set; }

        /// <summary>
        /// OPTIONAL. The identifier of the entity making the presentation (e.g., DID).
        /// </summary>
        [JsonPropertyName("holder")]
        public string Holder { get; set; }

        /// <summary>
        /// OPTIONAL. Audience or domain for which the presentation is intended (e.g., OIDC "aud").
        /// </summary>
        [JsonPropertyName("audience")]
        public string Audience { get; set; }

        /// <summary>
        /// OPTIONAL. Challenge value (e.g., nonce for anti-replay).
        /// </summary>
        [JsonPropertyName("challenge")]
        public string Challenge { get; set; }

        /// <summary>
        /// OPTIONAL. Domain for which the presentation is intended.
        /// </summary>
        [JsonPropertyName("domain")]
        public string Domain { get; set; }

        /// <summary>
        /// OPTIONAL. Expiration date for the presentation.
        /// </summary>
        [JsonPropertyName("expirationDate")]
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// OPTIONAL. Metadata for the presentation.
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; } = new();

        /// <summary>
        /// OPTIONAL. Linked Data Proof or JWT for the presentation itself.
        /// </summary>
        [JsonPropertyName("proof")]
        public object Proof { get; set; }

        /// <summary>
        /// OPTIONAL. Extension properties for ZKP, OIDC, custom flows, etc.
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> AdditionalProperties { get; set; } = new();

        


        /// <summary>
        /// Parses a JSON string into a VerifiablePresentation instance.
        /// </summary>
        public static VerifiablePresentation Parse(string json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("Input JSON is null or empty", nameof(json));

            try
            {
                var opts = GetJsonSerializerOptions();
                var vp = JsonSerializer.Deserialize<VerifiablePresentation>(json, opts);
                if (vp == null)
                    throw new JsonException("Could not parse VerifiablePresentation from JSON.");
                // (Optional) Validate minimal fields here, e.g. type/context
                if (vp.Type == null || vp.Type.Count == 0 || !vp.Type.Contains("VerifiablePresentation"))
                    throw new JsonException("Missing or invalid 'type' in VerifiablePresentation.");
                return vp;
            }
            catch (Exception ex)
            {
                throw new FormatException("Failed to parse VerifiablePresentation: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Serializes the VerifiablePresentation instance to a JSON string.
        /// </summary>
        public string ToJson(bool indented = false)
        {
            var opts = GetJsonSerializerOptions();
            opts.WriteIndented = indented;
            return JsonSerializer.Serialize(this, opts);
        }

        /// <summary>
        /// Returns all proofs as a list of JsonElement (handles object, array, or null).
        /// </summary>
        public IEnumerable<object> GetProofs()
        {
            if (Proof == null)
                return Array.Empty<object>();

            if (Proof is JsonElement elem)
            {
                if (elem.ValueKind == JsonValueKind.Array)
                    return elem.EnumerateArray().Cast<object>().ToList();
                if (elem.ValueKind == JsonValueKind.Object)
                    return new List<object> { elem };
                return Array.Empty<object>();
            }

            // If Proof is already a list (e.g., List<object>)
            if (Proof is IEnumerable<object> objList)
                return objList.ToList();

            // If Proof is a dictionary (e.g., Dictionary<string, object>)
            if (Proof is Dictionary<string, object> dict)
                return new List<object> { dict };

            // Fallback: try to serialize/deserialize to JsonElement
            var asJson = JsonSerializer.Serialize(Proof);
            var asElem = JsonSerializer.Deserialize<JsonElement>(asJson);
            if (asElem.ValueKind == JsonValueKind.Object)
                return new List<object> { asElem };
            if (asElem.ValueKind == JsonValueKind.Array)
                return asElem.EnumerateArray().Cast<object>().ToList();

            return Array.Empty<object>();
        }


        /// <summary>
        /// Provides configured JsonSerializerOptions for all VP operations.
        /// </summary>
        private static JsonSerializerOptions GetJsonSerializerOptions()
        {
            var opts = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new VerifiableCredentialListOrObjectConverter() },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            opts.Converters.Add(new JsonStringEnumConverter());
            // If you have custom converters for polymorphic credential types, add here.
            return opts;
        }
    }

    #region Custom Converter

    /// <summary>
    /// Handles verifiableCredential as either object, array, or JWT string.
    /// </summary>
    public class VerifiableCredentialListOrObjectConverter : JsonConverter<object>
    {
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.StartArray:
                    return JsonSerializer.Deserialize<List<object>>(ref reader, options);
                case JsonTokenType.StartObject:
                    return JsonSerializer.Deserialize<Dictionary<string, object>>(ref reader, options);
                case JsonTokenType.String:
                    return reader.GetString();
                default:
                    throw new JsonException("verifiableCredential must be object, array, or string (JWT/URI)");
            }
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case List<object> arr:
                    JsonSerializer.Serialize(writer, arr, options);
                    break;
                case Dictionary<string, object> obj:
                    JsonSerializer.Serialize(writer, obj, options);
                    break;
                case string jwt:
                    writer.WriteStringValue(jwt);
                    break;
                default:
                    JsonSerializer.Serialize(writer, value, options);
                    break;
            }
        }
    }

    #endregion
}