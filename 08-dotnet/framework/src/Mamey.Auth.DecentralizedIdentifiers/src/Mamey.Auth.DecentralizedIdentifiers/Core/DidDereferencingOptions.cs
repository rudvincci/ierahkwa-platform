namespace Mamey.Auth.DecentralizedIdentifiers.Core;

/// <summary>
/// Options for DID dereferencing.
/// </summary>
public class DidDereferencingOptions
{
    public string Accept { get; set; }
    public IDictionary<string, object> AdditionalOptions { get; set; }
}