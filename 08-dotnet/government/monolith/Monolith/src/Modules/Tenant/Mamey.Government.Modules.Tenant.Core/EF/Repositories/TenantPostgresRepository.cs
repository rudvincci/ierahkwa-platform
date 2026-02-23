using Mamey.Government.Modules.Tenant.Core.Domain.Entities;
using Mamey.Government.Modules.Tenant.Core.Domain.Repositories;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Tenant.Core.EF.Repositories;

internal class TenantPostgresRepository : ITenantRepository
{
    private readonly TenantDbContext _context;
    private readonly ILogger<TenantPostgresRepository> _logger;

    public TenantPostgresRepository(
        TenantDbContext context,
        ILogger<TenantPostgresRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TenantEntity?> GetAsync(TenantId id, CancellationToken cancellationToken = default)
    {
        var row = await _context.Tenants
            .FirstOrDefaultAsync(r => r.Id == id.Value, cancellationToken);
        
        return row?.AsEntity();
    }

    public async Task AddAsync(TenantEntity tenant, CancellationToken cancellationToken = default)
    {
        var row = tenant.AsRow();
        await _context.Tenants.AddAsync(row, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TenantEntity tenant, CancellationToken cancellationToken = default)
    {
        var row = await _context.Tenants
            .FirstOrDefaultAsync(r => r.Id == tenant.Id.Value, cancellationToken);
        
        if (row == null)
        {
            _logger.LogWarning("Tenant not found for update: {TenantId}", tenant.Id.Value);
            return;
        }

        row.UpdateFromEntity(tenant);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TenantId id, CancellationToken cancellationToken = default)
    {
        var row = await _context.Tenants
            .FirstOrDefaultAsync(r => r.Id == id.Value, cancellationToken);
        
        if (row != null)
        {
            _context.Tenants.Remove(row);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(TenantId id, CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .AnyAsync(r => r.Id == id.Value, cancellationToken);
    }

    public async Task<IReadOnlyList<TenantEntity>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _context.Tenants.ToListAsync(cancellationToken);
        return rows.Select(r => r.AsEntity()).ToList();
    }

    public async Task<TenantEntity?> GetByDomainAsync(string domain, CancellationToken cancellationToken = default)
    {
        var row = await _context.Tenants
            .FirstOrDefaultAsync(r => r.Domain == domain, cancellationToken);
        
        return row?.AsEntity();
    }
}

internal static class TenantRowExtensions
{
    public static TenantEntity AsEntity(this TenantRow row)
    {
        var tenantId = new TenantId(row.Id);
        var tenant = new TenantEntity(
            tenantId,
            row.DisplayName,
            row.Domain,
            row.IsActive,
            row.Version);
        
        // Use reflection to set private properties for read-only fields
        typeof(TenantEntity).GetProperty("CreatedAt")?.SetValue(tenant, row.CreatedAt);
        typeof(TenantEntity).GetProperty("UpdatedAt")?.SetValue(tenant, row.UpdatedAt);
        
        return tenant;
    }

    public static TenantRow AsRow(this TenantEntity entity)
    {
        return new TenantRow
        {
            Id = entity.Id.Value,
            DisplayName = entity.DisplayName,
            Domain = entity.Domain,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Version = entity.Version
        };
    }

    public static void UpdateFromEntity(this TenantRow row, TenantEntity entity)
    {
        row.DisplayName = entity.DisplayName;
        row.Domain = entity.Domain;
        row.IsActive = entity.IsActive;
        row.UpdatedAt = entity.UpdatedAt;
        row.Version = entity.Version;
    }
}
