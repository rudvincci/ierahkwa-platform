using System.Collections.Immutable;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Persistence.SQL.Repositories;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Government.Identity.Infrastructure.EF.Repositories;

internal class CredentialPostgresRepository : EFRepository<Credential, CredentialId>, ICredentialRepository, IDisposable
{
    private readonly UserDbContext _dbContext;

    public CredentialPostgresRepository(UserDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Credential>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var credentials = await _dbContext.Credentials.ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(credentials);
    }

    public async Task<Credential> GetAsync(CredentialId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Credentials
            .Where(c => c.Id.Value == id.Value)
            .SingleAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(CredentialId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Credentials
            .AnyAsync(c => c.Id.Value == id.Value, cancellationToken);
    }

    public async Task AddAsync(Credential credential, CancellationToken cancellationToken = default)
    {
        await _dbContext.Credentials.AddAsync(credential, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Credential credential, CancellationToken cancellationToken = default)
    {
        _dbContext.Credentials.Update(credential);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(CredentialId id, CancellationToken cancellationToken = default)
    {
        var credential = await _dbContext.Credentials
            .SingleAsync(c => c.Id.Value == id.Value, cancellationToken);
        _dbContext.Credentials.Remove(credential);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Credential-specific queries
    public async Task<IReadOnlyList<Credential>> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var credentials = await _dbContext.Credentials
            .Where(c => c.UserId.Value == userId.Value)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(credentials);
    }

    public async Task<IReadOnlyList<Credential>> GetByTypeAsync(CredentialType type, CancellationToken cancellationToken = default)
    {
        var credentials = await _dbContext.Credentials
            .Where(c => c.Type == type)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(credentials);
    }

    public async Task<IReadOnlyList<Credential>> GetByUserIdAndTypeAsync(UserId userId, CredentialType type, CancellationToken cancellationToken = default)
    {
        var credentials = await _dbContext.Credentials
            .Where(c => c.UserId.Value == userId.Value && c.Type == type)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(credentials);
    }

    public async Task<IReadOnlyList<Credential>> GetActiveCredentialsAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var credentials = await _dbContext.Credentials
            .Where(c => c.UserId.Value == userId.Value && c.Status == CredentialStatus.Active)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(credentials);
    }

    public async Task<bool> HasActiveCredentialOfTypeAsync(UserId userId, CredentialType type, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Credentials
            .AnyAsync(c => c.UserId.Value == userId.Value && c.Type == type && c.Status == CredentialStatus.Active, cancellationToken);
    }

    public async Task<int> GetActiveCredentialCountAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Credentials
            .CountAsync(c => c.UserId.Value == userId.Value && c.Status == CredentialStatus.Active, cancellationToken);
    }

    public async Task DeleteExpiredCredentialsAsync(CancellationToken cancellationToken = default)
    {
        var expiredCredentials = await _dbContext.Credentials
            .Where(c => c.ExpiresAt.HasValue && c.ExpiresAt.Value <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);
        
        _dbContext.Credentials.RemoveRange(expiredCredentials);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteCredentialsByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var userCredentials = await _dbContext.Credentials
            .Where(c => c.UserId.Value == userId.Value)
            .ToListAsync(cancellationToken);
        
        _dbContext.Credentials.RemoveRange(userCredentials);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteCredentialsByUserIdAndTypeAsync(UserId userId, CredentialType type, CancellationToken cancellationToken = default)
    {
        var userCredentials = await _dbContext.Credentials
            .Where(c => c.UserId.Value == userId.Value && c.Type == type)
            .ToListAsync(cancellationToken);
        
        _dbContext.Credentials.RemoveRange(userCredentials);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Credential>> GetByStatusAsync(CredentialStatus status, CancellationToken cancellationToken = default)
    {
        var credentials = await _dbContext.Credentials
            .Where(c => c.Status == status)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(credentials);
    }

    public async Task<IReadOnlyList<Credential>> GetExpiredCredentialsAsync(CancellationToken cancellationToken = default)
    {
        var credentials = await _dbContext.Credentials
            .Where(c => c.ExpiresAt.HasValue && c.ExpiresAt.Value <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(credentials);
    }

    public async Task<IReadOnlyList<Credential>> GetCredentialsExpiringSoonAsync(DateTime expirationThreshold, CancellationToken cancellationToken = default)
    {
        var credentials = await _dbContext.Credentials
            .Where(c => c.ExpiresAt.HasValue && c.ExpiresAt.Value <= expirationThreshold)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(credentials);
    }

    // Statistics and counting methods
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Credentials.CountAsync(cancellationToken);
    }

    public async Task<int> CountByStatusAsync(CredentialStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Credentials
            .CountAsync(c => c.Status == status, cancellationToken);
    }

    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }
        this.disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
