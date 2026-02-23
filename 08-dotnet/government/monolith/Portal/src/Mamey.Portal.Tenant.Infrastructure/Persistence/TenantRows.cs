namespace Mamey.Portal.Tenant.Infrastructure.Persistence;

public sealed class TenantRow
{
    public string TenantId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public sealed class TenantSettingsRow
{
    public string TenantId { get; set; } = string.Empty;
    public string BrandingJson { get; set; } = "{}";
    public string? ActivationJson { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public sealed class TenantDocumentNamingRow
{
    public string TenantId { get; set; } = string.Empty;
    public string PatternJson { get; set; } = "{}";
    public DateTimeOffset UpdatedAt { get; set; }
}

public sealed class TenantUserMappingRow
{
    public string Issuer { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public sealed class TenantUserInviteRow
{
    public string Issuer { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? UsedAt { get; set; }
}

public sealed class TenantDocumentTemplateRow
{
    public string TenantId { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public string TemplateHtml { get; set; } = string.Empty;
    public DateTimeOffset UpdatedAt { get; set; }
}

