using System.Text.RegularExpressions;
using Mamey.Portal.Shared.Auth;
using Mamey.Portal.Tenant.Application.Models;

namespace Mamey.Portal.Tenant.Application.Services;

public sealed class TenantUserInviteService : ITenantUserInviteService
{
    private static readonly Regex InvalidTenantChars = new(@"[^a-z0-9\-]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private readonly ITenantUserInviteStore _store;

    public TenantUserInviteService(ITenantUserInviteStore store)
    {
        _store = store;
    }

    public async Task<IReadOnlyList<TenantUserInvite>> GetByTenantAsync(string tenantId, CancellationToken ct = default)
    {
        tenantId = NormalizeTenantId(tenantId);
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            return Array.Empty<TenantUserInvite>();
        }

        return await _store.GetByTenantAsync(tenantId, ct);
    }

    public Task<TenantUserInvite?> GetAsync(string issuer, string email, CancellationToken ct = default)
    {
        issuer = NormalizeIssuer(issuer);
        email = NormalizeEmail(email);
        if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(email))
        {
            return Task.FromResult<TenantUserInvite?>(null);
        }

        return _store.GetAsync(issuer, email, ct);
    }

    public Task<TenantUserInvite> CreateOrUpdateAsync(
        string issuer,
        string email,
        string tenantId,
        CancellationToken ct = default)
    {
        issuer = NormalizeIssuer(issuer);
        email = NormalizeEmail(email);
        tenantId = NormalizeTenantId(tenantId);

        if (string.IsNullOrWhiteSpace(issuer)) throw new ArgumentException("issuer is required.");
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required.");
        if (string.IsNullOrWhiteSpace(tenantId)) throw new ArgumentException("tenantId is required.");

        return _store.UpsertAsync(issuer, email, tenantId, DateTimeOffset.UtcNow, ct);
    }

    public Task RevokeAsync(string issuer, string email, CancellationToken ct = default)
    {
        issuer = NormalizeIssuer(issuer);
        email = NormalizeEmail(email);
        if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(email))
        {
            return Task.CompletedTask;
        }

        return _store.DeleteAsync(issuer, email, ct);
    }

    private static string NormalizeIssuer(string issuer)
        => OidcIssuerNormalizer.Normalize(issuer);

    private static string NormalizeEmail(string email)
        => (email ?? string.Empty).Trim().ToLowerInvariant();

    private static string NormalizeTenantId(string tenantId)
    {
        tenantId = (tenantId ?? string.Empty).Trim().ToLowerInvariant();
        tenantId = tenantId.Replace(' ', '-');
        tenantId = InvalidTenantChars.Replace(tenantId, "");
        tenantId = tenantId.Trim('-');
        return tenantId.Length > 128 ? tenantId[..128] : tenantId;
    }
}
