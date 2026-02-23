using System.Security.Claims;

namespace Mamey.Templates;

/// <summary>Access context used in policy enforcement.</summary>
public sealed record TemplateAccessContext(
    string Service,
    string? TenantId = null,
    IReadOnlyCollection<Claim>? Claims = null);