using Mamey.Government.Modules.Certificates.Core.Domain.Entities;
using Mamey.Government.Modules.Certificates.Core.Domain.Repositories;
using Mamey.Government.Modules.Certificates.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Certificates.Core.EF.Repositories;

internal class CertificatePostgresRepository : ICertificateRepository
{
    private readonly CertificatesDbContext _context;
    private readonly ILogger<CertificatePostgresRepository> _logger;

    public CertificatePostgresRepository(
        CertificatesDbContext context,
        ILogger<CertificatePostgresRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Certificate?> GetAsync(CertificateId id, CancellationToken cancellationToken = default)
    {
        var row = await _context.Certificates
            .FirstOrDefaultAsync(r => r.Id == id.Value, cancellationToken);
        
        return row?.AsEntity();
    }

    public async Task AddAsync(Certificate certificate, CancellationToken cancellationToken = default)
    {
        var row = certificate.AsRow();
        await _context.Certificates.AddAsync(row, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Certificate certificate, CancellationToken cancellationToken = default)
    {
        var row = await _context.Certificates
            .FirstOrDefaultAsync(r => r.Id == certificate.Id.Value, cancellationToken);
        
        if (row == null)
        {
            _logger.LogWarning("Certificate not found for update: {CertificateId}", certificate.Id.Value);
            return;
        }

        row.UpdateFromEntity(certificate);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(CertificateId id, CancellationToken cancellationToken = default)
    {
        var row = await _context.Certificates
            .FirstOrDefaultAsync(r => r.Id == id.Value, cancellationToken);
        
        if (row != null)
        {
            _context.Certificates.Remove(row);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(CertificateId id, CancellationToken cancellationToken = default)
    {
        return await _context.Certificates
            .AnyAsync(r => r.Id == id.Value, cancellationToken);
    }

    public async Task<IReadOnlyList<Certificate>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _context.Certificates.ToListAsync(cancellationToken);
        return rows.Select(r => r.AsEntity()).ToList();
    }

    public async Task<Certificate?> GetByCertificateNumberAsync(string certificateNumber, CancellationToken cancellationToken = default)
    {
        var row = await _context.Certificates
            .FirstOrDefaultAsync(r => r.CertificateNumber == certificateNumber, cancellationToken);
        
        return row?.AsEntity();
    }

    public async Task<IReadOnlyList<Certificate>> GetByCitizenAsync(CitizenId citizenId, CancellationToken cancellationToken = default)
    {
        var rows = await _context.Certificates
            .Where(r => r.CitizenId == citizenId.Value)
            .ToListAsync(cancellationToken);
        return rows.Select(r => r.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Certificate>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default)
    {
        var rows = await _context.Certificates
            .Where(r => r.TenantId == tenantId.Value)
            .ToListAsync(cancellationToken);
        return rows.Select(r => r.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Certificate>> GetByTypeAsync(CertificateType certificateType, CancellationToken cancellationToken = default)
    {
        var rows = await _context.Certificates
            .Where(r => r.CertificateType == certificateType)
            .ToListAsync(cancellationToken);
        return rows.Select(r => r.AsEntity()).ToList();
    }
}

internal static class CertificateRowExtensions
{
    public static Certificate AsEntity(this CertificateRow row)
    {
        var certificateId = new CertificateId(row.Id);
        var tenantId = new TenantId(row.TenantId);
        // Certificate constructor expects Guid?, not CitizenId?, so convert properly handling null
        Guid? citizenIdGuid = row.CitizenId;
        
        var certificate = new Certificate(
            certificateId,
            tenantId,
            citizenIdGuid,
            row.CertificateType,
            row.CertificateNumber,
            row.IssuedDate,
            row.DocumentPath,
            row.Version);
        
        typeof(Certificate).GetProperty("IsActive")?.SetValue(certificate, row.IsActive);
        typeof(Certificate).GetProperty("RevokedAt")?.SetValue(certificate, row.RevokedAt);
        typeof(Certificate).GetProperty("RevocationReason")?.SetValue(certificate, row.RevocationReason);
        typeof(Certificate).GetProperty("CreatedAt")?.SetValue(certificate, row.CreatedAt);
        typeof(Certificate).GetProperty("UpdatedAt")?.SetValue(certificate, row.UpdatedAt);
        
        return certificate;
    }

    public static CertificateRow AsRow(this Certificate entity)
    {
        return new CertificateRow
        {
            Id = entity.Id.Value,
            TenantId = entity.TenantId.Value,
            CitizenId = entity.CitizenId,
            CertificateType = entity.CertificateType,
            CertificateNumber = entity.CertificateNumber,
            IssuedDate = entity.IssuedDate,
            DocumentPath = entity.DocumentPath,
            IsActive = entity.IsActive,
            RevokedAt = entity.RevokedAt,
            RevocationReason = entity.RevocationReason,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Version = entity.Version
        };
    }

    public static void UpdateFromEntity(this CertificateRow row, Certificate entity)
    {
        row.DocumentPath = entity.DocumentPath;
        row.IsActive = entity.IsActive;
        row.RevokedAt = entity.RevokedAt;
        row.RevocationReason = entity.RevocationReason;
        row.UpdatedAt = entity.UpdatedAt;
        row.Version = entity.Version;
    }
}
