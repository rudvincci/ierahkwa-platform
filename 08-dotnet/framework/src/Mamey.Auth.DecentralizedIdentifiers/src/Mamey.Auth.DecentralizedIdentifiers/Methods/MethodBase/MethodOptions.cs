namespace Mamey.Auth.DecentralizedIdentifiers.Methods.MethodBase;

/// <summary>
/// Base options class for DID method operations (for future extensibility).
/// </summary>
public class MethodOptions
{
    public IDictionary<string, object> Options { get; set; } = new Dictionary<string, object>();
}