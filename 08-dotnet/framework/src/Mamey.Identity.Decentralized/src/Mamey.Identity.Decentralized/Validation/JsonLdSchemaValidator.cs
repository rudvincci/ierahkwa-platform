using JsonLD.Core;
using Newtonsoft.Json.Linq;

namespace Mamey.Identity.Decentralized.Validation;

/// <summary>
/// Validates JSON-LD against DID schemas using JSON-LD framing.
/// </summary>
public static class JsonLdSchemaValidator
{
    /// <summary>
    /// Validates the DID Document's JSON-LD using the provided schema/context.
    /// Throws on fatal error, returns true if valid.
    /// </summary>
    public static async Task<bool> ValidateAsync(string jsonLd, string schemaContext,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(jsonLd)) throw new ArgumentNullException(nameof(jsonLd));
        if (string.IsNullOrWhiteSpace(schemaContext)) throw new ArgumentNullException(nameof(schemaContext));

        var input = JToken.Parse(jsonLd);
        var ctx = JToken.Parse(schemaContext);
        var options = new JsonLdOptions();
        var framed = JsonLdProcessor.Frame(input, ctx, options);
        // Check that required fields are present in the result
        if (!framed.ContainsKey("id"))
            throw new Exception("JSON-LD schema validation failed: missing 'id'.");
        return true;
    }
}