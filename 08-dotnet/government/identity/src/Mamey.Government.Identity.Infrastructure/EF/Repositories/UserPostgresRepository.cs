using System.Collections.Immutable;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Persistence.SQL.Repositories;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Government.Identity.Infrastructure.EF.Repositories;

internal class UserPostgresRepository : EFRepository<User, UserId>, IUserRepository, IDisposable
{
    private readonly UserDbContext _dbContext;

    public UserPostgresRepository(UserDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<User>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var users = await _dbContext.Users.ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(users);
    }

    public async Task<User> GetAsync(UserId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Where(u => u.Id == id)
            .SingleAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(UserId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AnyAsync(u => u.Id == id, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(UserId id, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .SingleAsync(u => u.Id.Value == id.Value, cancellationToken);
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Authentication-specific queries
    public async Task<User> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Where(u => u.Username == username)
            .SingleAsync(cancellationToken);
    }

    public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Where(u => u.Email == new Email(email))
            .SingleAsync(cancellationToken);
    }

    public async Task<User> GetBySubjectIdAsync(SubjectId subjectId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Where(u => u.SubjectId == subjectId)
            .SingleAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default)
    {
        var users = await _dbContext.Users
            .Where(u => u.Status == status)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(users);
    }

    public async Task<IReadOnlyList<User>> GetLockedUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _dbContext.Users
            .Where(u => u.Status == UserStatus.Locked)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(users);
    }

    public async Task<IReadOnlyList<User>> GetUsersWithFailedAttemptsAsync(int minAttempts, CancellationToken cancellationToken = default)
    {
        var users = await _dbContext.Users
            .Where(u => u.FailedLoginAttempts >= minAttempts)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(users);
    }

    public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AnyAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AnyAsync(u => u.Email.Value == email, cancellationToken);
    }

    public async Task<bool> SubjectIdExistsAsync(SubjectId subjectId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AnyAsync(u => u.SubjectId.Value == subjectId.Value, cancellationToken);
    }

    // Statistics and counting methods
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.CountAsync(cancellationToken);
    }

    public async Task<int> CountByStatusAsync(UserStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .CountAsync(u => u.Status == status, cancellationToken);
    }

    public async Task<int> CountLockedAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .CountAsync(u => u.Status == UserStatus.Locked, cancellationToken);
    }

    public async Task<int> CountWithTwoFactorAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .CountAsync(u => u.TwoFactorEnabled, cancellationToken);
    }

    public async Task<int> CountWithMultiFactorAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .CountAsync(u => u.MultiFactorEnabled, cancellationToken);
    }

    // Additional query methods
    public async Task<IReadOnlyList<User>> GetByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default)
    {
        // This would require joining with subjects and their roles
        // For now, return empty list - implement when needed
        return ImmutableList<User>.Empty;
    }

    public async Task<IReadOnlyList<User>> GetRecentlyAuthenticatedAsync(DateTime since, CancellationToken cancellationToken = default)
    {
        var users = await _dbContext.Users
            .Where(u => u.LastLoginAt.HasValue && u.LastLoginAt.Value >= since)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(users);
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
