using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Newtonsoft.Json.Linq;

namespace Mamey.Auth.DecentralizedIdentifiers.Core;

/// <summary>
/// Implements a W3C-compliant Service Endpoint for use in DID Documents.
/// </summary>
public class ServiceEndpoint : IDidServiceEndpoint
{
    public string Id { get; }
    public string Type { get; }
    public IReadOnlyList<object> Endpoints { get; }
    public IReadOnlyDictionary<string, object> AdditionalProperties { get; }

    public ServiceEndpoint(
        string id,
        string type,
        IEnumerable<object> serviceEndpoint,
        IDictionary<string, object> additionalProperties = null)
    {
        Id = id;
        Type = type;
        Endpoints = serviceEndpoint != null
            ? new List<object>(serviceEndpoint)
            : new List<object>();
        AdditionalProperties = additionalProperties != null
            ? new Dictionary<string, object>(additionalProperties)
            : new Dictionary<string, object>();
    }

    /// <summary>
    /// Parses a JObject into a ServiceEndpoint instance.
    /// </summary>
    /// <param name="obj">The JObject representing the service endpoint.</param>
    /// <returns>The parsed ServiceEndpoint.</returns>
    public static ServiceEndpoint Parse(JObject obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj), "Service endpoint JSON cannot be null.");

        // 1. id (required)
        string id = obj["id"]?.Value<string>()
                    ?? throw new FormatException("Service endpoint missing required 'id'.");

        // 2. type (required)
        string type = obj["type"]?.Value<string>()
                      ?? throw new FormatException("Service endpoint missing required 'type'.");

        // 3. serviceEndpoint (required, can be any JSON type, but typically string or array)
        var endpointToken = obj["serviceEndpoint"];
        if (endpointToken == null)
            throw new FormatException("Service endpoint missing required 'serviceEndpoint'.");

        List<object> endpoints;
        if (endpointToken.Type == JTokenType.Array)
        {
            endpoints = endpointToken.Select(ParseEndpointItem).ToList();
        }
        else
        {
            endpoints = new List<object> { ParseEndpointItem(endpointToken) };
        }

        // 4. Additional properties (for extensibility)
        var knownProps = new[] { "id", "type", "serviceEndpoint" };
        var additional = obj.Properties()
            .Where(p => !knownProps.Contains(p.Name))
            .ToDictionary(p => p.Name, p => (object)p.Value);

        return new ServiceEndpoint(id, type, endpoints, additional);
    }

    private static object ParseEndpointItem(JToken token)
    {
        // Commonly string or object, but spec allows anything
        switch (token.Type)
        {
            case JTokenType.String:
                return token.Value<string>();
            case JTokenType.Object:
                // Return as Dictionary for extensibility
                return token.ToObject<Dictionary<string, object>>();
            case JTokenType.Uri:
                return token.Value<string>();
            case JTokenType.Integer:
            case JTokenType.Float:
                return token.Value<double>();
            case JTokenType.Boolean:
                return token.Value<bool>();
            case JTokenType.Array:
                // Nested arrays are rare, but possible per JSON-LD flexibility
                return token.Select(ParseEndpointItem).ToList();
            default:
                return token.ToString();
        }
    }
    public JObject ToJsonObject()
    {
        var obj = new JObject
        {
            ["id"] = Id,
            ["type"] = Type
        };

        // serviceEndpoint is usually an array or single value
        if (Endpoints != null)
        {
            if (Endpoints.Count == 1)
            {
                obj["serviceEndpoint"] = JToken.FromObject(Endpoints[0]);
            }
            else
            {
                obj["serviceEndpoint"] = new JArray(Endpoints.Select(JToken.FromObject));
            }
        }

        // Add any additional/extensible properties
        if (AdditionalProperties != null)
        {
            foreach (var prop in AdditionalProperties)
            {
                // Prevent overwriting standard properties
                if (!obj.ContainsKey(prop.Key))
                    obj[prop.Key] = prop.Value is JToken jt ? jt : JToken.FromObject(prop.Value);
            }
        }

        return obj;
    }
    public string ToJson()
    {
        return ToJsonObject().ToString();
    }
}