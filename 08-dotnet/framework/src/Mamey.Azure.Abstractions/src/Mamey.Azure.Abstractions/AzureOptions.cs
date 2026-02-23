namespace Mamey.Azure.Abstractions;

public class AzureOptions
{
    public const string APPSETTINGS_SECTION = "azureAd";

    public AzureOptions()
    {

    }

    public bool Enabled { get; set; }
    public string? Type { get; set; }
    public string? Auth { get; set; }
    public string? Instance { get; set; }
    public string? TenantId { get; set; }
    public string? Domain { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? Authority { get; set; }
    public string? Scopes { get; set; }
    public string? CallbackPath { get; set; }
    public DownstreamApi DownstreamApi { get; set; }
}
public class AzureBlazorWasmOptions : AzureOptions
{

    public string? RedirectUri { get; set; }
}

public class DownstreamApi
{
    public string? BaseUrl { get; set; }
    public string? Scopes { get; set; }
}