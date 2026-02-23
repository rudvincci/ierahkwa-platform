namespace Mamey.Identity.Decentralized.Configuration;

/// <summary>
/// High-level DID resolution configuration (timeouts, cache, preferred methods).
/// </summary>
public class DidResolutionOptions
{
    public TimeSpan ResolutionTimeout { get; set; } = TimeSpan.FromSeconds(15);
    public List<string> PreferredMethods { get; set; } = new() { "ion", "web" };
    public bool EnableCache { get; set; } = true;
    public int CacheSize { get; set; } = 500;
}