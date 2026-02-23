using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Users.Domain.Entities;
using Pupitre.Users.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Users.Infrastructure.PostgreSQL.Repositories;

internal class UserPostgresRepository : EFRepository<User, UserId>, IUserRepository, IDisposable
{
    private readonly UsersDbContext _serviceNameDbContext;
    public UserPostgresRepository(UsersDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<User>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<User> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.Users.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<User> GetAsync(UserId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.Users
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(User entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.Users.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(UserId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.Users.Any(c => c.Id == id.Value));
    public async Task UpdateAsync(User entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.Users.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(User entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(UserId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.Users.Single(c => c.Id == id.Value), cancellationToken);
    }
    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _serviceNameDbContext.Dispose();
            }
        }
        this.disposed = true;
    }
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}