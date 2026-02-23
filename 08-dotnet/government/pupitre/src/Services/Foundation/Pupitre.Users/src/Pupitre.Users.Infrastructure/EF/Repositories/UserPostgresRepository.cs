using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Users.Domain.Entities;
using Pupitre.Users.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Users.Infrastructure.EF.Repositories;

internal class UserPostgresRepository : EFRepository<User, UserId>, IUserRepository, IDisposable
{
    private readonly UserDbContext _entityNameDbContext;
    public UserPostgresRepository(UserDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<User>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<User> entityNames = ImmutableList.CreateRange(_entityNameDbContext.Users.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<User> GetAsync(UserId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.Users
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(User entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.Users.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(UserId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.Users.Any(c => c.Id == id.Value));
    public async Task UpdateAsync(User entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.Users.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(User entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(UserId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.Users.Single(c => c.Id == id.Value), cancellationToken);
    }
    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _entityNameDbContext.Dispose();
            }
        }
        this.disposed = true;
    }
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}