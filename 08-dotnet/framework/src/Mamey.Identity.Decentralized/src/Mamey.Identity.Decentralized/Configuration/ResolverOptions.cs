namespace Mamey.Identity.Decentralized.Configuration;

public class ResolverOptions
{
    public List<string> EnabledMethods { get; set; } = new() { "ion", "web", "pkh", "key" };
    public Dictionary<string, string> MethodEndpoints { get; set; } = new()
    {
        { "ion", "https://ion.tbd.network/" },
        { "web", "https://did-web-resolver.example.com/" }
    };
    public bool AllowUnregisteredMethods { get; set; } = false;
    
}