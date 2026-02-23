namespace Mamey.Portal.Tenant.Application.Models;

public sealed record TenantUserMapping(
    string Issuer,
    string Subject,
    string TenantId,
    string? Email,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);




