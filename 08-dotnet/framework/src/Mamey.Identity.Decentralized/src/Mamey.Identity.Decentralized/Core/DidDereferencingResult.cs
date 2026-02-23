namespace Mamey.Identity.Decentralized.Core;

/// <summary>
/// Represents the result of dereferencing a DID URL.
/// </summary>
public class DidDereferencingResult
{
    public object Content { get; set; }
    public string ContentType { get; set; }
    public IDictionary<string, object> DereferencingMetadata { get; set; }
    public IDictionary<string, object> ResolutionMetadata { get; set; }
}