using Mamey.Portal.Citizenship.Application.Models;
using Mamey.Portal.Shared.Auth;
using Mamey.Portal.Shared.Tenancy;

namespace Mamey.Portal.Citizenship.Application.Services;

public sealed class CitizenshipCitizenPortalService : ICitizenPortalService
{
    private readonly ICitizenPortalStore _store;
    private readonly ITenantContext _tenant;
    private readonly ICurrentUserContext _user;

    public CitizenshipCitizenPortalService(
        ICitizenPortalStore store,
        ITenantContext tenant,
        ICurrentUserContext user)
    {
        _store = store;
        _tenant = tenant;
        _user = user;
    }

    public async Task<CitizenProfileDto?> GetProfileAsync(CancellationToken ct = default)
    {
        if (!_user.IsAuthenticated || string.IsNullOrWhiteSpace(_user.UserName))
        {
            return null;
        }

        var email = _user.UserName.Trim();
        var tenantId = _tenant.TenantId;
        var latest = await _store.GetLatestProfileAsync(tenantId, email, ct);

        if (latest is null)
        {
            // No application yet: keep it privacy-safe and minimal.
            return new CitizenProfileDto(
                CitizenId: $"CIT-{tenantId.ToUpperInvariant()}-UNVERIFIED",
                FullName: "(not yet verified)",
                DateOfBirth: new DateOnly(1900, 1, 1),
                Email: email);
        }

        return new CitizenProfileDto(
            CitizenId: $"CIT-{tenantId.ToUpperInvariant()}-UNVERIFIED",
            FullName: $"{latest.FirstName} {latest.LastName}",
            DateOfBirth: latest.DateOfBirth,
            Email: latest.Email ?? email);
    }

    public async Task<IReadOnlyList<CitizenApplicationDto>> GetApplicationsAsync(CancellationToken ct = default)
    {
        if (!_user.IsAuthenticated || string.IsNullOrWhiteSpace(_user.UserName))
        {
            return Array.Empty<CitizenApplicationDto>();
        }

        var email = _user.UserName.Trim();
        var tenantId = _tenant.TenantId;

        return await _store.GetApplicationsAsync(tenantId, email, ct);
    }

    public async Task<IReadOnlyList<CitizenDocumentDto>> GetDocumentsAsync(CancellationToken ct = default)
    {
        if (!_user.IsAuthenticated || string.IsNullOrWhiteSpace(_user.UserName))
        {
            return Array.Empty<CitizenDocumentDto>();
        }

        var email = _user.UserName.Trim();
        var tenantId = _tenant.TenantId;
        var docs = await _store.GetDocumentsAsync(tenantId, email, ct);

        return docs.Select(x =>
        {
            var (type, variant) = MapKind(x.Kind);
            return new CitizenDocumentDto(
                IssuedDocumentId: x.Id,
                DocumentNumber: x.DocumentNumber ?? "(pending)",
                Type: type,
                Variant: variant,
                IssuedAt: x.CreatedAt,
                ExpiresAt: x.ExpiresAt);
        }).ToList();
    }

    public async Task<IReadOnlyList<CitizenUploadDto>> GetUploadsAsync(CancellationToken ct = default)
    {
        if (!_user.IsAuthenticated || string.IsNullOrWhiteSpace(_user.UserName))
        {
            return Array.Empty<CitizenUploadDto>();
        }

        var email = _user.UserName.Trim();
        var tenantId = _tenant.TenantId;

        return await _store.GetUploadsAsync(tenantId, email, ct);
    }

    private static (string Type, string Variant) MapKind(string kind)
    {
        if (string.Equals(kind, "Passport", StringComparison.OrdinalIgnoreCase))
        {
            return ("Passport", "Standard");
        }

        if (kind.StartsWith("IdCard:", StringComparison.OrdinalIgnoreCase))
        {
            return ("ID Card", kind["IdCard:".Length..]);
        }

        if (kind.StartsWith("VehicleTag:", StringComparison.OrdinalIgnoreCase))
        {
            return ("Vehicle Tag", kind["VehicleTag:".Length..]);
        }

        if (string.Equals(kind, "CitizenshipCertificate", StringComparison.OrdinalIgnoreCase))
        {
            return ("Certificate", "Citizenship");
        }

        return ("Document", kind);
    }
}
