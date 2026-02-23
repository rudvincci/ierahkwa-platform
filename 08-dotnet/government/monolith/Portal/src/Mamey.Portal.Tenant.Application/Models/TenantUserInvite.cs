namespace Mamey.Portal.Tenant.Application.Models;

public sealed record TenantUserInvite(
    string Issuer,
    string Email,
    string TenantId,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? UsedAt);
