namespace Mamey.Portal.Tenant.Application.Services;

public sealed record TenantRowSnapshot(
    string TenantId,
    string DisplayName,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record TenantSettingsSnapshot(
    string TenantId,
    string? BrandingJson,
    string? ActivationJson,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record TenantDocumentNamingSnapshot(
    string TenantId,
    string? PatternJson,
    DateTimeOffset UpdatedAt);
