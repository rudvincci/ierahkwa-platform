using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Types;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.Entities;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.Repositories;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.TravelIdentities.Core.EF.Repositories;

internal class TravelIdentityPostgresRepository : ITravelIdentityRepository
{
    private readonly TravelIdentitiesDbContext _context;
    private readonly ILogger<TravelIdentityPostgresRepository> _logger;

    public TravelIdentityPostgresRepository(
        TravelIdentitiesDbContext context,
        ILogger<TravelIdentityPostgresRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TravelIdentity?> GetAsync(TravelIdentityId id, CancellationToken cancellationToken = default)
    {
        var row = await _context.TravelIdentities
            .FirstOrDefaultAsync(r => r.Id == id.Value, cancellationToken);
        
        return row?.AsEntity();
    }

    public async Task AddAsync(TravelIdentity travelIdentity, CancellationToken cancellationToken = default)
    {
        var row = travelIdentity.AsRow();
        await _context.TravelIdentities.AddAsync(row, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TravelIdentity travelIdentity, CancellationToken cancellationToken = default)
    {
        var row = await _context.TravelIdentities
            .FirstOrDefaultAsync(r => r.Id == travelIdentity.Id.Value, cancellationToken);
        
        if (row == null)
        {
            _logger.LogWarning("TravelIdentity not found for update: {TravelIdentityId}", travelIdentity.Id.Value);
            return;
        }

        row.UpdateFromEntity(travelIdentity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TravelIdentityId id, CancellationToken cancellationToken = default)
    {
        var row = await _context.TravelIdentities
            .FirstOrDefaultAsync(r => r.Id == id.Value, cancellationToken);
        
        if (row != null)
        {
            _context.TravelIdentities.Remove(row);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(TravelIdentityId id, CancellationToken cancellationToken = default)
    {
        return await _context.TravelIdentities
            .AnyAsync(r => r.Id == id.Value, cancellationToken);
    }

    public async Task<IReadOnlyList<TravelIdentity>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _context.TravelIdentities.ToListAsync(cancellationToken);
        return rows.Select(r => r.AsEntity()).ToList();
    }

    public async Task<TravelIdentity?> GetByTravelIdentityNumberAsync(TravelIdentityNumber travelIdentityNumber, CancellationToken cancellationToken = default)
    {
        var row = await _context.TravelIdentities
            .FirstOrDefaultAsync(r => r.TravelIdentityNumber == travelIdentityNumber.Value, cancellationToken);
        
        return row?.AsEntity();
    }

    public async Task<IReadOnlyList<TravelIdentity>> GetByCitizenAsync(CitizenId citizenId, CancellationToken cancellationToken = default)
    {
        var rows = await _context.TravelIdentities
            .Where(r => r.CitizenId == citizenId.Value)
            .ToListAsync(cancellationToken);
        return rows.Select(r => r.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<TravelIdentity>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default)
    {
        var rows = await _context.TravelIdentities
            .Where(r => r.TenantId == tenantId.Value)
            .ToListAsync(cancellationToken);
        return rows.Select(r => r.AsEntity()).ToList();
    }

    public async Task<int> GetCountByYearAsync(TenantId tenantId, int year, CancellationToken cancellationToken = default)
    {
        return await _context.TravelIdentities
            .Where(r => r.TenantId == tenantId.Value && r.IssuedDate.Year == year)
            .CountAsync(cancellationToken);
    }
}

internal static class TravelIdentityRowExtensions
{
    public static TravelIdentity AsEntity(this TravelIdentityRow row)
    {
        var travelIdentityId = new TravelIdentityId(row.Id);
        var tenantId = new TenantId(row.TenantId);
        var citizenId = new CitizenId(row.CitizenId);
        var travelIdentityNumber = new TravelIdentityNumber(row.TravelIdentityNumber);
        
        var travelIdentity = new TravelIdentity(
            travelIdentityId,
            tenantId,
            citizenId,
            travelIdentityNumber,
            row.IssuedDate,
            row.ExpiryDate,
            row.Pdf417Barcode,
            row.Version);
        
        typeof(TravelIdentity).GetProperty("DocumentPath")?.SetValue(travelIdentity, row.DocumentPath);
        typeof(TravelIdentity).GetProperty("IsActive")?.SetValue(travelIdentity, row.IsActive);
        typeof(TravelIdentity).GetProperty("RevokedAt")?.SetValue(travelIdentity, row.RevokedAt);
        typeof(TravelIdentity).GetProperty("RevocationReason")?.SetValue(travelIdentity, row.RevocationReason);
        typeof(TravelIdentity).GetProperty("CreatedAt")?.SetValue(travelIdentity, row.CreatedAt);
        typeof(TravelIdentity).GetProperty("UpdatedAt")?.SetValue(travelIdentity, row.UpdatedAt);
        
        return travelIdentity;
    }

    public static TravelIdentityRow AsRow(this TravelIdentity entity)
    {
        return new TravelIdentityRow
        {
            Id = entity.Id.Value,
            TenantId = entity.TenantId.Value,
            CitizenId = entity.CitizenId,
            TravelIdentityNumber = entity.TravelIdentityNumber.Value,
            IssuedDate = entity.IssuedDate,
            ExpiryDate = entity.ExpiryDate,
            Pdf417Barcode = entity.Pdf417Barcode,
            DocumentPath = entity.DocumentPath,
            IsActive = entity.IsActive,
            RevokedAt = entity.RevokedAt,
            RevocationReason = entity.RevocationReason,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Version = entity.Version
        };
    }

    public static void UpdateFromEntity(this TravelIdentityRow row, TravelIdentity entity)
    {
        row.ExpiryDate = entity.ExpiryDate;
        row.Pdf417Barcode = entity.Pdf417Barcode;
        row.DocumentPath = entity.DocumentPath;
        row.IsActive = entity.IsActive;
        row.RevokedAt = entity.RevokedAt;
        row.RevocationReason = entity.RevocationReason;
        row.UpdatedAt = entity.UpdatedAt;
        row.Version = entity.Version;
    }
}
