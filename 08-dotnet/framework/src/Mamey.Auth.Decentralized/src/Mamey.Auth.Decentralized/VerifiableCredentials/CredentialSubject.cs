using System.Text.Json.Serialization;
using Mamey.Auth.Decentralized.Exceptions;

namespace Mamey.Auth.Decentralized.VerifiableCredentials;

/// <summary>
/// Represents a credential subject as defined by W3C Verifiable Credentials Data Model 1.1
/// </summary>
public class CredentialSubject
{
    /// <summary>
    /// The unique identifier of the subject
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// The type of the subject
    /// </summary>
    [JsonPropertyName("type")]
    public List<string>? Type { get; set; }

    /// <summary>
    /// Additional properties of the subject
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> Properties { get; set; } = new();

    /// <summary>
    /// Validates the credential subject structure
    /// </summary>
    /// <returns>True if the subject is valid, false otherwise</returns>
    public bool IsValid()
    {
        try
        {
            // At least one property must be present
            return Properties.Count > 0 || !string.IsNullOrEmpty(Id);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets a property value by name
    /// </summary>
    /// <param name="name">The property name</param>
    /// <returns>The property value, or null if not found</returns>
    public JsonElement? GetProperty(string name)
    {
        return Properties.TryGetValue(name, out var value) ? value : null;
    }

    /// <summary>
    /// Sets a property value by name
    /// </summary>
    /// <param name="name">The property name</param>
    /// <param name="value">The property value</param>
    public void SetProperty(string name, JsonElement value)
    {
        Properties[name] = value;
    }

    /// <summary>
    /// Removes a property by name
    /// </summary>
    /// <param name="name">The property name</param>
    /// <returns>True if the property was removed, false if not found</returns>
    public bool RemoveProperty(string name)
    {
        return Properties.Remove(name);
    }

    /// <summary>
    /// Checks if a property exists
    /// </summary>
    /// <param name="name">The property name</param>
    /// <returns>True if the property exists, false otherwise</returns>
    public bool HasProperty(string name)
    {
        return Properties.ContainsKey(name);
    }

    /// <summary>
    /// Gets all property names
    /// </summary>
    /// <returns>List of property names</returns>
    public List<string> GetPropertyNames()
    {
        return Properties.Keys.ToList();
    }

    /// <summary>
    /// Creates a copy of the credential subject
    /// </summary>
    /// <returns>A copy of the credential subject</returns>
    public CredentialSubject Clone()
    {
        return new CredentialSubject
        {
            Id = Id,
            Type = Type?.ToList(),
            Properties = new Dictionary<string, JsonElement>(Properties)
        };
    }
}
