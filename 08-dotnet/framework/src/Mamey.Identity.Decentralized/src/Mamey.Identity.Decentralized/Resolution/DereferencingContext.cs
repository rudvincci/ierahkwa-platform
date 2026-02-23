namespace Mamey.Identity.Decentralized.Resolution;

/// <summary>
/// Provides advanced context and options for dereferencing operations.
/// </summary>
public class DereferencingContext
{
    public IDictionary<string, object> Options { get; set; } = new Dictionary<string, object>();
    public IDictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
}