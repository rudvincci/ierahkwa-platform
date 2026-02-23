using Mamey.Types;

namespace Mamey.Government.Modules.Tenant.Core.Domain.Entities;

/// <summary>
/// Tenant-specific settings and configuration.
/// </summary>
internal class TenantSettings
{
    private TenantSettings() { }

    public TenantSettings(
        TenantId tenantId,
        string brandingJson = "{}",
        string? activationJson = null,
        string? documentNamingConfig = null)
    {
        TenantId = tenantId;
        BrandingJson = brandingJson;
        ActivationJson = activationJson;
        DocumentNamingConfig = documentNamingConfig;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public TenantId TenantId { get; private set; }
    public string BrandingJson { get; private set; } = "{}";
    public string? ActivationJson { get; private set; }
    public string? DocumentNamingConfig { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public void UpdateBranding(string brandingJson)
    {
        BrandingJson = brandingJson;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateActivation(string? activationJson)
    {
        ActivationJson = activationJson;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDocumentNaming(string? documentNamingConfig)
    {
        DocumentNamingConfig = documentNamingConfig;
        UpdatedAt = DateTime.UtcNow;
    }
}
