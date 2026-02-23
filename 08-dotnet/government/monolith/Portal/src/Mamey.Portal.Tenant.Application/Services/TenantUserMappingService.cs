using Mamey.Portal.Shared.Auth;
using Mamey.Portal.Tenant.Application.Models;

namespace Mamey.Portal.Tenant.Application.Services;

public sealed class TenantUserMappingService : ITenantUserMappingService
{
    private readonly ITenantUserMappingStore _store;

    public TenantUserMappingService(ITenantUserMappingStore store)
    {
        _store = store;
    }

    public Task<IReadOnlyList<TenantUserMapping>> GetAllAsync(int take = 200, CancellationToken ct = default)
    {
        take = take <= 0 ? 200 : Math.Min(take, 1000);
        return _store.GetAllAsync(take, ct);
    }

    public Task<TenantUserMapping?> GetAsync(string issuer, string subject, CancellationToken ct = default)
    {
        issuer = OidcIssuerNormalizer.Normalize(issuer);
        subject = (subject ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(subject)) return Task.FromResult<TenantUserMapping?>(null);

        return _store.GetAsync(issuer, subject, ct);
    }

    public Task<TenantUserMapping> UpsertAsync(
        string issuer,
        string subject,
        string tenantId,
        string? email,
        CancellationToken ct = default)
    {
        issuer = OidcIssuerNormalizer.Normalize(issuer);
        subject = (subject ?? string.Empty).Trim();
        tenantId = (tenantId ?? string.Empty).Trim();
        email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();

        if (string.IsNullOrWhiteSpace(issuer)) throw new ArgumentException("issuer is required.");
        if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentException("subject is required.");
        if (string.IsNullOrWhiteSpace(tenantId)) throw new ArgumentException("tenantId is required.");

        return _store.UpsertAsync(issuer, subject, tenantId, email, DateTimeOffset.UtcNow, ct);
    }

    public Task DeleteAsync(string issuer, string subject, CancellationToken ct = default)
    {
        issuer = OidcIssuerNormalizer.Normalize(issuer);
        subject = (subject ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(subject)) return Task.CompletedTask;

        return _store.DeleteAsync(issuer, subject, ct);
    }
}
