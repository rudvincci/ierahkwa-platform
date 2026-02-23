namespace SpikeOffice.Core.Entities;

/// <summary>
/// Multi-tenant: each tenant has a custom URL prefix / workspace.
/// IERAHKWA: sovereign departments can be tenants.
/// </summary>
public class Tenant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    /// <summary>URL prefix / workspace: e.g. "ierahkwa-pm", "ierahkwa-mft"</summary>
    public string UrlPrefix { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? PrimaryColor { get; set; }
    public string? DefaultLanguage { get; set; } = "en";
    public string? TimeZone { get; set; } = "UTC";
    public bool IsActive { get; set; } = true;
    
    // IERAHKWA: link to government department / IGT if needed
    public string? IerahkwaDepartmentCode { get; set; }
    public string? IgtTokenSymbol { get; set; }

    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
