namespace Mamey.Types;

public class AppOptions
{
    public AppOptions(){ }
    public AppOptions(string? id, Guid? organizationId, string? name, string service, string instance, string version, 
        bool displayBanner, bool displayVersion, string? url, string? serverClientConnectionName, string? domain, 
        string? clientId, string? webClientUrl, string? tenantId)
    {
        Id = id;
        OrganizationId = organizationId;
        Name = name;
        Service = service;
        Instance = instance;
        Version = version;
        DisplayBanner = displayBanner;
        DisplayVersion = displayVersion;
        Url = url;
        ServerClientConnectionName = serverClientConnectionName;
        Domain = domain;
        ClientId = clientId;
        WebClientUrl = webClientUrl;
        TenantId = tenantId;
    }

    public string? Id { get; set; }
    public Guid? OrganizationId { get; set; }
    public string? Name { get; set; }
    public string Service { get; set; }
    public string Instance { get; set; }
    public string Version { get; set; }
    public bool DisplayBanner { get; set; } = true;
    public bool DisplayVersion { get; set; } = true;
    public string? Url { get; set; }
    public string? ServerClientConnectionName { get; set; }
    public string? Domain { get; set; }
    public string? ClientId { get; set; }
    public string? WebClientUrl { get; set; }
    public string? SiteTitle { get; set; }
    public string? TenantId { get; set; }
}
