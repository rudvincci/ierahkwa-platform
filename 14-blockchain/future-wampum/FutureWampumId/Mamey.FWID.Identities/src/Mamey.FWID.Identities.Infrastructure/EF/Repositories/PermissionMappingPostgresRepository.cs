using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Mamey.FWID.Identities.Infrastructure.EF.Repositories;

/// <summary>
/// PostgreSQL repository implementation for PermissionMapping entities.
/// </summary>
internal class PermissionMappingPostgresRepository : IPermissionMappingRepository
{
    private readonly IdentityDbContext _dbContext;

    public PermissionMappingPostgresRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task AddAsync(PermissionMapping mapping, CancellationToken cancellationToken = default)
    {
        await _dbContext.PermissionMappings.AddAsync(mapping, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(PermissionMapping mapping, CancellationToken cancellationToken = default)
    {
        _dbContext.PermissionMappings.Update(mapping);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PermissionMapping?> GetByServiceNameAsync(string serviceName, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PermissionMappings
            .FirstOrDefaultAsync(p => p.ServiceName == serviceName, cancellationToken);
    }

    public async Task<PermissionMapping?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PermissionMappings
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<PermissionMapping>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.PermissionMappings
            .Where(p => p.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var mapping = await GetByIdAsync(id, cancellationToken);
        if (mapping != null)
        {
            mapping.Deactivate();
            await UpdateAsync(mapping, cancellationToken);
        }
    }
}

