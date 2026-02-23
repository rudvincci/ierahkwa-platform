using System.Text.Json;
using System.Text.Json.Nodes;
using Mamey.Auth.DecentralizedIdentifiers.Utilities;

namespace Mamey.Auth.DecentralizedIdentifiers.Serialization;

/// <summary>
/// Provides helpers to add, replace, and extract Linked Data Proofs in JSON-LD objects
/// according to the W3C Verifiable Credential and Presentation data model.
/// </summary>
public static class LinkedDataProofSerializer
{
    /// <summary>
    /// Adds or replaces a "proof" property on a JSON-LD object.
    /// Supports multiple proofs (array) or a single proof (object).
    /// </summary>
    /// <param name="jsonLd">The original JSON-LD as JsonElement.</param>
    /// <param name="proof">The proof object (can be JsonElement, Dictionary, or POCO).</param>
    /// <returns>A new JsonElement with the proof property added or replaced.</returns>
    public static JsonElement AddOrReplaceProof(JsonElement jsonLd, object proof)
    {
        using var doc = AddOrReplaceProofInternal(jsonLd, proof);
        return doc.RootElement.Clone();
    }

    private static JsonDocument AddOrReplaceProofInternal(JsonElement jsonLd, object proof)
    {
        var jsonObj = JsonNode.Parse(jsonLd.GetRawText()) as JsonObject;
        if (jsonObj == null) throw new InvalidOperationException("Input JSON-LD is not a JSON object.");

        // Handle multi-proof (array) or single-proof (object)
        if (proof is IEnumerable<object> arr && !(proof is string))
        {
            var arrNode = new JsonArray();
            foreach (var p in arr)
                arrNode.Add(JsonNode.Parse(JsonSerializer.Serialize(p)));
            jsonObj["proof"] = arrNode;
        }
        else
        {
            jsonObj["proof"] = JsonNode.Parse(JsonSerializer.Serialize(proof));
        }

        return JsonDocument.Parse(jsonObj.ToJsonString());
    }

    /// <summary>
    /// Extracts the proof(s) property from a JSON-LD object.
    /// </summary>
    /// <param name="jsonLd">The JSON-LD as JsonElement.</param>
    /// <returns>The proof property (JsonElement), or null if not present.</returns>
    public static JsonElement? ExtractProof(JsonElement jsonLd)
    {
        if (jsonLd.ValueKind != JsonValueKind.Object)
            return null;

        if (jsonLd.TryGetProperty("proof", out var proofProp))
            return proofProp;

        return null;
    }

    /// <summary>
    /// Removes the proof property from the JSON-LD object and returns a new JsonElement.
    /// (Utility provided for symmetry with AddOrReplaceProof.)
    /// </summary>
    /// <param name="jsonLd">The JSON-LD as JsonElement.</param>
    /// <returns>New JsonElement with proof removed.</returns>
    public static JsonElement RemoveProof(JsonElement jsonLd)
    {
        return JsonUtils.RemoveProofProperty(jsonLd);
    }
}