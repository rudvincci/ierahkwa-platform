using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mamey.Auth.Decentralized.Utilities;

/// <summary>
/// Utility class for JSON-LD processing
/// </summary>
public static class JsonLdProcessor
{
    private static readonly JsonSerializerOptions JsonLdOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        PropertyNameCaseInsensitive = true,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
    };
    
    /// <summary>
    /// Expands a JSON-LD document using the provided context
    /// </summary>
    /// <param name="document">The JSON-LD document to expand</param>
    /// <param name="context">The context to use for expansion</param>
    /// <returns>Expanded JSON-LD document</returns>
    public static string Expand(string document, string context)
    {
        if (string.IsNullOrEmpty(document))
            throw new ArgumentException("Document cannot be null or empty", nameof(document));
        
        if (string.IsNullOrEmpty(context))
            throw new ArgumentException("Context cannot be null or empty", nameof(context));
        
        var doc = JsonDocument.Parse(document);
        var ctx = JsonDocument.Parse(context);
        
        return Expand(doc, ctx);
    }
    
    /// <summary>
    /// Expands a JSON-LD document using the provided context
    /// </summary>
    /// <param name="document">The JSON-LD document to expand</param>
    /// <param name="context">The context to use for expansion</param>
    /// <returns>Expanded JSON-LD document</returns>
    public static string Expand(JsonDocument document, JsonDocument context)
    {
        if (document == null)
            throw new ArgumentNullException(nameof(document));
        
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        
        var expanded = new Dictionary<string, object>();
        var contextObj = context.RootElement;
        
        // Process the document with the context
        ProcessElement(document.RootElement, contextObj, expanded);
        
        return JsonSerializer.Serialize(expanded, JsonLdOptions);
    }
    
    /// <summary>
    /// Compacts a JSON-LD document using the provided context
    /// </summary>
    /// <param name="document">The JSON-LD document to compact</param>
    /// <param name="context">The context to use for compaction</param>
    /// <returns>Compacted JSON-LD document</returns>
    public static string Compact(string document, string context)
    {
        if (string.IsNullOrEmpty(document))
            throw new ArgumentException("Document cannot be null or empty", nameof(document));
        
        if (string.IsNullOrEmpty(context))
            throw new ArgumentException("Context cannot be null or empty", nameof(context));
        
        var doc = JsonDocument.Parse(document);
        var ctx = JsonDocument.Parse(context);
        
        return Compact(doc, ctx);
    }
    
    /// <summary>
    /// Compacts a JSON-LD document using the provided context
    /// </summary>
    /// <param name="document">The JSON-LD document to compact</param>
    /// <param name="context">The context to use for compaction</param>
    /// <returns>Compacted JSON-LD document</returns>
    public static string Compact(JsonDocument document, JsonDocument context)
    {
        if (document == null)
            throw new ArgumentNullException(nameof(document));
        
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        
        var compacted = new Dictionary<string, object>();
        var contextObj = context.RootElement;
        
        // Process the document with the context
        ProcessElement(document.RootElement, contextObj, compacted);
        
        return JsonSerializer.Serialize(compacted, JsonLdOptions);
    }
    
    /// <summary>
    /// Flattens a JSON-LD document
    /// </summary>
    /// <param name="document">The JSON-LD document to flatten</param>
    /// <returns>Flattened JSON-LD document</returns>
    public static string Flatten(string document)
    {
        if (string.IsNullOrEmpty(document))
            throw new ArgumentException("Document cannot be null or empty", nameof(document));
        
        var doc = JsonDocument.Parse(document);
        return Flatten(doc);
    }
    
    /// <summary>
    /// Flattens a JSON-LD document
    /// </summary>
    /// <param name="document">The JSON-LD document to flatten</param>
    /// <returns>Flattened JSON-LD document</returns>
    public static string Flatten(JsonDocument document)
    {
        if (document == null)
            throw new ArgumentNullException(nameof(document));
        
        var flattened = new Dictionary<string, object>();
        var nodes = new Dictionary<string, object>();
        
        // Process the document to extract nodes
        ProcessElementForFlattening(document.RootElement, nodes);
        
        // Add the nodes to the flattened result
        foreach (var node in nodes)
        {
            flattened[node.Key] = node.Value;
        }
        
        return JsonSerializer.Serialize(flattened, JsonLdOptions);
    }
    
    /// <summary>
    /// Processes a JsonElement for expansion or compaction
    /// </summary>
    /// <param name="element">The element to process</param>
    /// <param name="context">The context to use</param>
    /// <param name="result">The result dictionary</param>
    private static void ProcessElement(JsonElement element, JsonElement context, Dictionary<string, object> result)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                ProcessObject(element, context, result);
                break;
                
            case JsonValueKind.Array:
                var array = new List<object>();
                foreach (var item in element.EnumerateArray())
                {
                    var itemObj = new Dictionary<string, object>();
                    ProcessElement(item, context, itemObj);
                    array.Add(itemObj);
                }
                result["@value"] = array;
                break;
                
            default:
                result["@value"] = GetElementValue(element);
                break;
        }
    }
    
    /// <summary>
    /// Processes a JSON object for expansion or compaction
    /// </summary>
    /// <param name="obj">The object to process</param>
    /// <param name="context">The context to use</param>
    /// <param name="result">The result dictionary</param>
    private static void ProcessObject(JsonElement obj, JsonElement context, Dictionary<string, object> result)
    {
        foreach (var property in obj.EnumerateObject())
        {
            var key = property.Name;
            var value = property.Value;
            
            // Handle special JSON-LD keywords
            if (key.StartsWith("@"))
            {
                result[key] = GetElementValue(value);
            }
            else
            {
                // Process the property value
                var processedValue = ProcessPropertyValue(value, context);
                result[key] = processedValue;
            }
        }
    }
    
    /// <summary>
    /// Processes a property value for expansion or compaction
    /// </summary>
    /// <param name="value">The value to process</param>
    /// <param name="context">The context to use</param>
    /// <returns>The processed value</returns>
    private static object ProcessPropertyValue(JsonElement value, JsonElement context)
    {
        return value.ValueKind switch
        {
            JsonValueKind.Object => ProcessObjectValue(value, context),
            JsonValueKind.Array => ProcessArrayValue(value, context),
            _ => GetElementValue(value)
        };
    }
    
    /// <summary>
    /// Processes an object value
    /// </summary>
    /// <param name="obj">The object to process</param>
    /// <param name="context">The context to use</param>
    /// <returns>The processed object</returns>
    private static object ProcessObjectValue(JsonElement obj, JsonElement context)
    {
        var result = new Dictionary<string, object>();
        ProcessObject(obj, context, result);
        return result;
    }
    
    /// <summary>
    /// Processes an array value
    /// </summary>
    /// <param name="array">The array to process</param>
    /// <param name="context">The context to use</param>
    /// <returns>The processed array</returns>
    private static object ProcessArrayValue(JsonElement array, JsonElement context)
    {
        var result = new List<object>();
        foreach (var item in array.EnumerateArray())
        {
            var itemObj = new Dictionary<string, object>();
            ProcessElement(item, context, itemObj);
            result.Add(itemObj);
        }
        return result;
    }
    
    /// <summary>
    /// Processes an element for flattening
    /// </summary>
    /// <param name="element">The element to process</param>
    /// <param name="nodes">The nodes dictionary</param>
    private static void ProcessElementForFlattening(JsonElement element, Dictionary<string, object> nodes)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            // Check if this is a node with an @id
            if (element.TryGetProperty("@id", out var idElement) && idElement.ValueKind == JsonValueKind.String)
            {
                var id = idElement.GetString()!;
                if (!nodes.ContainsKey(id))
                {
                    var node = new Dictionary<string, object>();
                    ProcessObjectForFlattening(element, node);
                    nodes[id] = node;
                }
            }
            else
            {
                // Process all properties
                foreach (var property in element.EnumerateObject())
                {
                    ProcessElementForFlattening(property.Value, nodes);
                }
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in element.EnumerateArray())
            {
                ProcessElementForFlattening(item, nodes);
            }
        }
    }
    
    /// <summary>
    /// Processes an object for flattening
    /// </summary>
    /// <param name="obj">The object to process</param>
    /// <param name="result">The result dictionary</param>
    private static Dictionary<string, object> ProcessObjectForFlattening(JsonElement obj, Dictionary<string, object> result)
    {
        foreach (var property in obj.EnumerateObject())
        {
            var key = property.Name;
            var value = property.Value;
            
            if (key.StartsWith("@"))
            {
                result[key] = GetElementValue(value);
            }
            else
            {
                result[key] = ProcessPropertyValueForFlattening(value);
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// Processes a property value for flattening
    /// </summary>
    /// <param name="value">The value to process</param>
    /// <returns>The processed value</returns>
    private static object ProcessPropertyValueForFlattening(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.Object => ProcessObjectForFlattening(value, new Dictionary<string, object>()),
            JsonValueKind.Array => ProcessArrayForFlattening(value),
            _ => GetElementValue(value)
        };
    }
    
    /// <summary>
    /// Processes an array for flattening
    /// </summary>
    /// <param name="array">The array to process</param>
    /// <returns>The processed array</returns>
    private static object ProcessArrayForFlattening(JsonElement array)
    {
        var result = new List<object>();
        foreach (var item in array.EnumerateArray())
        {
            result.Add(ProcessPropertyValueForFlattening(item));
        }
        return result;
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
            _ => element
        };
    }
}
