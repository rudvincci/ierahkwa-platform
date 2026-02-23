using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Citizenship.Application.Models;
using Mamey.Portal.Citizenship.Application.Services;
using Mamey.Portal.Citizenship.Infrastructure.Persistence;

namespace Mamey.Portal.Citizenship.Infrastructure.Services;

public sealed class CitizenPortalStore : ICitizenPortalStore
{
    private readonly CitizenshipDbContext _db;

    public CitizenPortalStore(CitizenshipDbContext db)
    {
        _db = db;
    }

    public async Task<CitizenPortalProfileSnapshot?> GetLatestProfileAsync(string tenantId, string email, CancellationToken ct = default)
    {
        return await _db.Applications
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.Email == email)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new CitizenPortalProfileSnapshot(
                x.FirstName,
                x.LastName,
                x.DateOfBirth,
                x.Email))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<CitizenApplicationDto>> GetApplicationsAsync(string tenantId, string email, CancellationToken ct = default)
    {
        return await _db.Applications
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.Email == email)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new CitizenApplicationDto(
                x.ApplicationNumber,
                x.Status,
                x.CreatedAt))
            .Take(50)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<CitizenPortalDocumentSnapshot>> GetDocumentsAsync(string tenantId, string email, CancellationToken ct = default)
    {
        return await (
                from d in _db.IssuedDocuments.AsNoTracking()
                join a in _db.Applications.AsNoTracking() on d.ApplicationId equals a.Id
                where a.TenantId == tenantId && a.Email == email
                orderby d.CreatedAt descending
                select new CitizenPortalDocumentSnapshot(
                    d.Id,
                    d.Kind,
                    d.DocumentNumber,
                    d.CreatedAt,
                    d.ExpiresAt))
            .Take(200)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<CitizenUploadDto>> GetUploadsAsync(string tenantId, string email, CancellationToken ct = default)
    {
        return await (
                from u in _db.Uploads.AsNoTracking()
                join a in _db.Applications.AsNoTracking() on u.ApplicationId equals a.Id
                where a.TenantId == tenantId && a.Email == email
                orderby u.UploadedAt descending
                select new CitizenUploadDto(u.Id, u.Kind, u.FileName, u.ContentType, u.Size, u.UploadedAt))
            .Take(200)
            .ToListAsync(ct);
    }
}

