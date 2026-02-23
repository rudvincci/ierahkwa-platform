using System.Text.Json.Serialization;

namespace Mamey.Auth.Decentralized.Core;

/// <summary>
/// Represents a service endpoint in a DID Document as defined by W3C DID 1.1 specification.
/// Service endpoints provide information about services associated with the DID subject.
/// </summary>
public class ServiceEndpoint
{
    /// <summary>
    /// The unique identifier for this service endpoint
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// The type of service endpoint
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    /// <summary>
    /// The service endpoint URL or identifier
    /// </summary>
    [JsonPropertyName("serviceEndpoint")]
    public string ServiceEndpointUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Additional properties specific to the service type
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> Properties { get; set; } = new();
    
    /// <summary>
    /// Validates the service endpoint according to W3C DID 1.1 specification
    /// </summary>
    /// <returns>True if the service endpoint is valid, false otherwise</returns>
    public bool IsValid()
    {
        // Check required fields
        if (string.IsNullOrEmpty(Id))
            return false;
        
        if (string.IsNullOrEmpty(Type))
            return false;
        
        if (string.IsNullOrEmpty(ServiceEndpointUrl))
            return false;
        
        // Validate URL format if it's a URL
        if (ServiceEndpointUrl.StartsWith("http://") || ServiceEndpointUrl.StartsWith("https://"))
        {
            if (!Uri.TryCreate(ServiceEndpointUrl, UriKind.Absolute, out var uri))
                return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Gets a property value by key
    /// </summary>
    /// <param name="key">The property key</param>
    /// <returns>The property value if found, null otherwise</returns>
    public JsonElement? GetProperty(string key)
    {
        return Properties.TryGetValue(key, out var value) ? value : null;
    }
    
    /// <summary>
    /// Sets a property value
    /// </summary>
    /// <param name="key">The property key</param>
    /// <param name="value">The property value</param>
    public void SetProperty(string key, object value)
    {
        var jsonElement = JsonSerializer.SerializeToElement(value);
        Properties[key] = jsonElement;
    }
    
    /// <summary>
    /// Removes a property
    /// </summary>
    /// <param name="key">The property key to remove</param>
    /// <returns>True if the property was removed, false if not found</returns>
    public bool RemoveProperty(string key)
    {
        return Properties.Remove(key);
    }
    
    /// <summary>
    /// Creates a service endpoint for a specific service type
    /// </summary>
    /// <param name="id">The service endpoint ID</param>
    /// <param name="type">The service type</param>
    /// <param name="serviceEndpointUrl">The service endpoint URL</param>
    /// <param name="properties">Additional properties</param>
    /// <returns>A new ServiceEndpoint instance</returns>
    public static ServiceEndpoint Create(string id, string type, string serviceEndpointUrl, Dictionary<string, object>? properties = null)
    {
        var service = new ServiceEndpoint
        {
            Id = id,
            Type = type,
            ServiceEndpointUrl = serviceEndpointUrl
        };
        
        if (properties != null)
        {
            foreach (var kvp in properties)
            {
                service.SetProperty(kvp.Key, kvp.Value);
            }
        }
        
        return service;
    }
    
    /// <summary>
    /// Creates a DIDComm service endpoint
    /// </summary>
    /// <param name="id">The service endpoint ID</param>
    /// <param name="serviceEndpointUrl">The service endpoint URL</param>
    /// <param name="routingKeys">Optional routing keys</param>
    /// <param name="accept">Optional accepted media types</param>
    /// <returns>A new ServiceEndpoint instance for DIDComm</returns>
    public static ServiceEndpoint CreateDidComm(string id, string serviceEndpointUrl, List<string>? routingKeys = null, List<string>? accept = null)
    {
        var service = Create(id, "DIDCommMessaging", serviceEndpointUrl);
        
        if (routingKeys != null && routingKeys.Any())
        {
            service.SetProperty("routingKeys", routingKeys);
        }
        
        if (accept != null && accept.Any())
        {
            service.SetProperty("accept", accept);
        }
        
        return service;
    }
    
    /// <summary>
    /// Creates a Linked Data service endpoint
    /// </summary>
    /// <param name="id">The service endpoint ID</param>
    /// <param name="serviceEndpointUrl">The service endpoint URL</param>
    /// <param name="origins">Optional allowed origins</param>
    /// <returns>A new ServiceEndpoint instance for Linked Data</returns>
    public static ServiceEndpoint CreateLinkedData(string id, string serviceEndpointUrl, List<string>? origins = null)
    {
        var service = Create(id, "LinkedDomains", serviceEndpointUrl);
        
        if (origins != null && origins.Any())
        {
            service.SetProperty("origins", origins);
        }
        
        return service;
    }
    
    /// <summary>
    /// Creates a Web service endpoint
    /// </summary>
    /// <param name="id">The service endpoint ID</param>
    /// <param name="serviceEndpointUrl">The service endpoint URL</param>
    /// <param name="description">Optional description</param>
    /// <returns>A new ServiceEndpoint instance for Web</returns>
    public static ServiceEndpoint CreateWeb(string id, string serviceEndpointUrl, string? description = null)
    {
        var service = Create(id, "Web", serviceEndpointUrl);
        
        if (!string.IsNullOrEmpty(description))
        {
            service.SetProperty("description", description);
        }
        
        return service;
    }
}
