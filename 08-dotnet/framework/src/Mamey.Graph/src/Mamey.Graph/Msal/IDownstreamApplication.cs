namespace Mamey.Graph.Msal;
public interface IDownstreamApplication
{
    public string? Scopes { get; set; }
    public string? BaseUrl { get; set; }
}