using Mamey.Portal.Shared.Storage.DocumentNaming;

namespace Mamey.Portal.Tenant.Application.Models;

public sealed record TenantSummary(
    string TenantId,
    string DisplayName,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record TenantBranding(
    string DisplayName,
    string? SealLine1,
    string? SealLine2,
    string? ContactEmail);

public sealed record TenantActivationChecklist(
    bool BrandingComplete,
    bool NamingComplete,
    bool TemplatesComplete,
    bool AdminMembershipEstablished,
    bool FeatureFlagsReviewed)
{
    public static TenantActivationChecklist Empty { get; } = new(false, false, false, false, false);

    public bool IsComplete
        => BrandingComplete
           && NamingComplete
           && TemplatesComplete
           && AdminMembershipEstablished
           && FeatureFlagsReviewed;
}

public sealed record TenantSettings(
    string TenantId,
    TenantBranding Branding,
    DocumentNamingPattern DocumentNaming,
    TenantActivationChecklist ActivationChecklist);



