using Mamey.Azure.Abstractions;

namespace Mamey.Graph;

public class GraphOptions : AzureOptions
{
    public string? BaseUrl { get; set; }
    public string? Instance { get; set; }
    public List<DownstreamApp>? DownstreamApps { get; set; }

}
    
public class MameyPublicClientOptions : GraphOptions
{
    public MameyPublicClientOptions()
    {
        
    }

    public string? SignedOutCallBackPath { get; set; }
    public string? SignUpSignInPolicyId { get; set; }
    public string? ResetPasswordPolicyId { get; set; }
    public string? EditProfilePolicyId { get; set; }
    public string? SignedOutCallbackPath { get; set; }
    public string? RedirectUri { get; set; }
}
public class MameyConfidentialClientOptions : GraphOptions
{

}
public class DownstreamApp
{
    public string? Name { get; set; }
    public string? Scopes { get; set; }
}
//public class OPt : Microsoft.Graph.GraphClientOptions