using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mamey.Auth.Decentralized.Utilities;

/// <summary>
/// Utility class for JSON-LD canonicalization
/// </summary>
public static class Canonicalizer
{
    private static readonly JsonSerializerOptions CanonicalOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        PropertyNameCaseInsensitive = false,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
    };
    
    /// <summary>
    /// Canonicalizes a JSON object according to JSON-LD specification
    /// </summary>
    /// <param name="obj">The object to canonicalize</param>
    /// <returns>Canonicalized JSON string</returns>
    public static string Canonicalize(object obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));
        
        // Convert to JSON
        var json = JsonSerializer.Serialize(obj, CanonicalOptions);
        
        // Parse and re-serialize to ensure consistent ordering
        var document = JsonDocument.Parse(json);
        return CanonicalizeJsonDocument(document);
    }
    
    /// <summary>
    /// Canonicalizes a JSON string according to JSON-LD specification
    /// </summary>
    /// <param name="json">The JSON string to canonicalize</param>
    /// <returns>Canonicalized JSON string</returns>
    public static string Canonicalize(string json)
    {
        if (string.IsNullOrEmpty(json))
            throw new ArgumentException("JSON string cannot be null or empty", nameof(json));
        
        var document = JsonDocument.Parse(json);
        return CanonicalizeJsonDocument(document);
    }
    
    /// <summary>
    /// Canonicalizes a JsonDocument according to JSON-LD specification
    /// </summary>
    /// <param name="document">The JsonDocument to canonicalize</param>
    /// <returns>Canonicalized JSON string</returns>
    public static string CanonicalizeJsonDocument(JsonDocument document)
    {
        if (document == null)
            throw new ArgumentNullException(nameof(document));
        
        var canonical = new Dictionary<string, object>();
        CanonicalizeElement(document.RootElement, canonical);
        
        return JsonSerializer.Serialize(canonical, CanonicalOptions);
    }
    
    /// <summary>
    /// Recursively canonicalizes a JsonElement
    /// </summary>
    /// <param name="element">The JsonElement to canonicalize</param>
    /// <param name="result">The result dictionary</param>
    private static void CanonicalizeElement(JsonElement element, Dictionary<string, object> result)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var obj = new Dictionary<string, object>();
                foreach (var property in element.EnumerateObject().OrderBy(p => p.Name))
                {
                    CanonicalizeElement(property.Value, obj);
                    obj[property.Name] = GetElementValue(property.Value);
                }
                result["@type"] = "object";
                result["@value"] = obj;
                break;
                
            case JsonValueKind.Array:
                var array = new List<object>();
                foreach (var item in element.EnumerateArray())
                {
                    var itemObj = new Dictionary<string, object>();
                    CanonicalizeElement(item, itemObj);
                    array.Add(GetElementValue(item));
                }
                result["@type"] = "array";
                result["@value"] = array;
                break;
                
            case JsonValueKind.String:
                result["@type"] = "string";
                result["@value"] = element.GetString()!;
                break;
                
            case JsonValueKind.Number:
                result["@type"] = "number";
                result["@value"] = element.GetDecimal();
                break;
                
            case JsonValueKind.True:
                result["@type"] = "boolean";
                result["@value"] = true;
                break;
                
            case JsonValueKind.False:
                result["@type"] = "boolean";
                result["@value"] = false;
                break;
                
            case JsonValueKind.Null:
                result["@type"] = "null";
                result["@value"] = null;
                break;
        }
    }
    
    /// <summary>
    /// Gets the value of a JsonElement
    /// </summary>
    /// <param name="element">The JsonElement</param>
    /// <returns>The element value</returns>
    private static object GetElementValue(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString()!,
            JsonValueKind.Number => element.GetDecimal(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null!,
            JsonValueKind.Object => element,
            JsonValueKind.Array => element,
            _ => throw new ArgumentException($"Unsupported JsonValueKind: {element.ValueKind}")
        };
    }
    
    /// <summary>
    /// Normalizes a JSON-LD document for comparison
    /// </summary>
    /// <param name="json">The JSON-LD document</param>
    /// <returns>Normalized JSON string</returns>
    public static string Normalize(string json)
    {
        if (string.IsNullOrEmpty(json))
            throw new ArgumentException("JSON string cannot be null or empty", nameof(json));
        
        var document = JsonDocument.Parse(json);
        var normalized = new Dictionary<string, object>();
        
        // Process the root object
        if (document.RootElement.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in document.RootElement.EnumerateObject().OrderBy(p => p.Name))
            {
                normalized[property.Name] = GetElementValue(property.Value);
            }
        }
        
        return JsonSerializer.Serialize(normalized, CanonicalOptions);
    }
}
