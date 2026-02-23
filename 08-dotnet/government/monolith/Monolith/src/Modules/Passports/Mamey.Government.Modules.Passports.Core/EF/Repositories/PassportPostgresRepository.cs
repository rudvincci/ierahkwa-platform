using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Passports.Core.Domain.Entities;
using Mamey.Government.Modules.Passports.Core.Domain.Repositories;
using Mamey.Government.Modules.Passports.Core.Domain.ValueObjects;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Passports.Core.EF.Repositories;

internal class PassportPostgresRepository : IPassportRepository
{
    private readonly PassportsDbContext _context;
    private readonly ILogger<PassportPostgresRepository> _logger;

    public PassportPostgresRepository(
        PassportsDbContext context,
        ILogger<PassportPostgresRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Passport?> GetAsync(PassportId id, CancellationToken cancellationToken = default)
    {
        var row = await _context.Passports
            .FirstOrDefaultAsync(r => r.Id == id.Value, cancellationToken);
        
        return row?.AsEntity();
    }

    public async Task AddAsync(Passport passport, CancellationToken cancellationToken = default)
    {
        var row = passport.AsRow();
        await _context.Passports.AddAsync(row, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Passport passport, CancellationToken cancellationToken = default)
    {
        var row = await _context.Passports
            .FirstOrDefaultAsync(r => r.Id == passport.Id.Value, cancellationToken);
        
        if (row == null)
        {
            _logger.LogWarning("Passport not found for update: {PassportId}", passport.Id.Value);
            return;
        }

        row.UpdateFromEntity(passport);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(PassportId id, CancellationToken cancellationToken = default)
    {
        var row = await _context.Passports
            .FirstOrDefaultAsync(r => r.Id == id.Value, cancellationToken);
        
        if (row != null)
        {
            _context.Passports.Remove(row);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(PassportId id, CancellationToken cancellationToken = default)
    {
        return await _context.Passports
            .AnyAsync(r => r.Id == id.Value, cancellationToken);
    }

    public async Task<IReadOnlyList<Passport>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _context.Passports.ToListAsync(cancellationToken);
        return rows.Select(r => r.AsEntity()).ToList();
    }

    public async Task<Passport?> GetByPassportNumberAsync(PassportNumber passportNumber, CancellationToken cancellationToken = default)
    {
        var row = await _context.Passports
            .FirstOrDefaultAsync(r => r.PassportNumber == passportNumber.Value, cancellationToken);
        
        return row?.AsEntity();
    }

    public async Task<IReadOnlyList<Passport>> GetByCitizenAsync(CitizenId citizenId, CancellationToken cancellationToken = default)
    {
        var rows = await _context.Passports
            .Where(r => r.CitizenId == citizenId.Value)
            .ToListAsync(cancellationToken);
        return rows.Select(r => r.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Passport>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default)
    {
        var rows = await _context.Passports
            .Where(r => r.TenantId == tenantId.Value)
            .ToListAsync(cancellationToken);
        return rows.Select(r => r.AsEntity()).ToList();
    }
}

internal static class PassportRowExtensions
{
    public static Passport AsEntity(this PassportRow row)
    {
        var passportId = new PassportId(row.Id);
        var tenantId = new TenantId(row.TenantId);
        var citizenId = new CitizenId(row.CitizenId);
        var passportNumber = new PassportNumber(row.PassportNumber);
        
        var passport = new Passport(
            passportId,
            tenantId,
            citizenId,
            passportNumber,
            row.IssuedDate,
            row.ExpiryDate,
            row.Mrz,
            row.Version);
        
        typeof(Passport).GetProperty("DocumentPath")?.SetValue(passport, row.DocumentPath);
        typeof(Passport).GetProperty("IsActive")?.SetValue(passport, row.IsActive);
        typeof(Passport).GetProperty("RevokedAt")?.SetValue(passport, row.RevokedAt);
        typeof(Passport).GetProperty("RevocationReason")?.SetValue(passport, row.RevocationReason);
        typeof(Passport).GetProperty("CreatedAt")?.SetValue(passport, row.CreatedAt);
        typeof(Passport).GetProperty("UpdatedAt")?.SetValue(passport, row.UpdatedAt);
        
        return passport;
    }

    public static PassportRow AsRow(this Passport entity)
    {
        return new PassportRow
        {
            Id = entity.Id.Value,
            TenantId = entity.TenantId.Value,
            CitizenId = entity.CitizenId,
            PassportNumber = entity.PassportNumber.Value,
            IssuedDate = entity.IssuedDate,
            ExpiryDate = entity.ExpiryDate,
            Mrz = entity.Mrz,
            DocumentPath = entity.DocumentPath,
            IsActive = entity.IsActive,
            RevokedAt = entity.RevokedAt,
            RevocationReason = entity.RevocationReason,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Version = entity.Version
        };
    }

    public static void UpdateFromEntity(this PassportRow row, Passport entity)
    {
        row.ExpiryDate = entity.ExpiryDate;
        row.Mrz = entity.Mrz;
        row.DocumentPath = entity.DocumentPath;
        row.IsActive = entity.IsActive;
        row.RevokedAt = entity.RevokedAt;
        row.RevocationReason = entity.RevocationReason;
        row.UpdatedAt = entity.UpdatedAt;
        row.Version = entity.Version;
    }
}
