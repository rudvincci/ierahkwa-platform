using Mamey.Government.Modules.Citizens.Core.Domain.Entities;
using Mamey.Government.Modules.Citizens.Core.Domain.Repositories;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Mamey.Government.Modules.Citizens.Core.EF.Repositories;

internal class CitizenPostgresRepository : ICitizenRepository
{
    private readonly CitizensDbContext _context;
    private readonly ILogger<CitizenPostgresRepository> _logger;

    public CitizenPostgresRepository(
        CitizensDbContext context,
        ILogger<CitizenPostgresRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Citizen?> GetAsync(CitizenId id, CancellationToken cancellationToken = default)
    {
        var row = await _context.Citizens
            .Include(c => c.StatusHistory)
            .FirstOrDefaultAsync(r => r.Id == id.Value, cancellationToken);
        
        return row?.AsEntity();
    }

    public async Task AddAsync(Citizen citizen, CancellationToken cancellationToken = default)
    {
        var row = citizen.AsRow();
        await _context.Citizens.AddAsync(row, cancellationToken);
        
        // Add status history entries
        foreach (var history in citizen.StatusHistory)
        {
            await _context.StatusHistory.AddAsync(history.AsRow(citizen.Id.Value), cancellationToken);
        }
        
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Citizen citizen, CancellationToken cancellationToken = default)
    {
        var row = await _context.Citizens
            .FirstOrDefaultAsync(r => r.Id == citizen.Id.Value, cancellationToken);
        
        if (row == null)
        {
            _logger.LogWarning("Citizen not found for update: {CitizenId}", citizen.Id.Value);
            return;
        }

        row.UpdateFromEntity(citizen);
        
        // Update status history if changed
        var latestHistory = citizen.StatusHistory.OrderByDescending(h => h.ChangedAt).FirstOrDefault();
        if (latestHistory != null)
        {
            var existingHistory = await _context.StatusHistory
                .FirstOrDefaultAsync(h => h.CitizenId == citizen.Id.Value && h.ChangedAt == latestHistory.ChangedAt, cancellationToken);
            
            if (existingHistory == null)
            {
                await _context.StatusHistory.AddAsync(latestHistory.AsRow(citizen.Id.Value), cancellationToken);
            }
        }
        
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(CitizenId id, CancellationToken cancellationToken = default)
    {
        var row = await _context.Citizens
            .FirstOrDefaultAsync(r => r.Id == id.Value, cancellationToken);
        
        if (row != null)
        {
            _context.Citizens.Remove(row);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(CitizenId id, CancellationToken cancellationToken = default)
    {
        return await _context.Citizens
            .AnyAsync(r => r.Id == id.Value, cancellationToken);
    }

    public async Task<IReadOnlyList<Citizen>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _context.Citizens
            .Include(c => c.StatusHistory)
            .ToListAsync(cancellationToken);
        return rows.Select(r => r.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Citizen>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default)
    {
        var rows = await _context.Citizens
            .Include(c => c.StatusHistory)
            .Where(r => r.TenantId == tenantId.Value)
            .ToListAsync(cancellationToken);
        return rows.Select(r => r.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Citizen>> GetByStatusAsync(CitizenshipStatus status, CancellationToken cancellationToken = default)
    {
        var rows = await _context.Citizens
            .Include(c => c.StatusHistory)
            .Where(r => r.Status == status)
            .ToListAsync(cancellationToken);
        return rows.Select(r => r.AsEntity()).ToList();
    }
}

internal static class CitizenRowExtensions
{
    public static Citizen AsEntity(this CitizenRow row)
    {
        var citizenId = new CitizenId(row.Id);
        var tenantId = new TenantId(row.TenantId);
        var citizenName = new Name(row.FirstName, row.LastName);
        var email = string.IsNullOrEmpty(row.Email) ? null : new Email(row.Email);
        Phone? phone = null;
        if (!string.IsNullOrEmpty(row.Phone))
        {
            phone = new Phone("1", row.Phone);
        }
        var address = string.IsNullOrEmpty(row.AddressJson) ? null : JsonSerializer.Deserialize<Address>(row.AddressJson);
        
        var citizen = new Citizen(
            citizenId,
            tenantId,
            citizenName,
            email,
            phone,
            address,
            row.DateOfBirth,
            row.Status);
        
        typeof(Citizen).GetProperty("PhotoPath")?.SetValue(citizen, row.PhotoPath);
        typeof(Citizen).GetProperty("CreatedAt")?.SetValue(citizen, row.CreatedAt);
        typeof(Citizen).GetProperty("UpdatedAt")?.SetValue(citizen, row.UpdatedAt);
        
        return citizen;
    }

    public static CitizenRow AsRow(this Citizen entity)
    {
        return new CitizenRow
        {
            Id = entity.Id.Value,
            TenantId = entity.TenantId.Value,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email?.ToString(),
            Phone = entity.Phone?.ToString(),
            AddressJson = entity.Address != null ? JsonSerializer.Serialize(entity.Address) : null,
            DateOfBirth = entity.DateOfBirth,
            Status = entity.Status,
            PhotoPath = entity.PhotoPath,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Version = entity.Version
        };
    }

    public static void UpdateFromEntity(this CitizenRow row, Citizen entity)
    {
        row.FirstName = entity.FirstName;
        row.LastName = entity.LastName;
        row.Email = entity.Email?.ToString();
        row.Phone = entity.Phone?.ToString();
        row.AddressJson = entity.Address != null ? JsonSerializer.Serialize(entity.Address) : null;
        row.DateOfBirth = entity.DateOfBirth;
        row.Status = entity.Status;
        row.PhotoPath = entity.PhotoPath;
        row.UpdatedAt = entity.UpdatedAt;
        row.Version = entity.Version;
    }
}

internal static class StatusHistoryRowExtensions
{
    public static CitizenshipStatusHistory AsEntity(this CitizenshipStatusHistoryRow row)
    {
        return new CitizenshipStatusHistory(row.Status, row.ChangedAt, row.Reason);
    }

    public static CitizenshipStatusHistoryRow AsRow(this CitizenshipStatusHistory history, Guid citizenId)
    {
        return new CitizenshipStatusHistoryRow
        {
            CitizenId = citizenId,
            Status = history.Status,
            ChangedAt = history.ChangedAt,
            Reason = history.Reason
        };
    }
}
