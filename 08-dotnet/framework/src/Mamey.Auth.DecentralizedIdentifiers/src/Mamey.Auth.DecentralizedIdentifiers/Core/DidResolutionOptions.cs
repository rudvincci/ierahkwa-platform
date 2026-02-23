namespace Mamey.Auth.DecentralizedIdentifiers.Core;

// <summary>
/// Options for DID resolution.
/// </summary>
public class DidResolutionOptions
{
    public string Accept { get; set; }
    public IDictionary<string, object> AdditionalOptions { get; set; }
}