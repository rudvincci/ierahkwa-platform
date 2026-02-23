using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Notifications.Domain.Entities;
using Pupitre.Notifications.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Notifications.Infrastructure.EF.Repositories;

internal class NotificationPostgresRepository : EFRepository<Notification, NotificationId>, INotificationRepository, IDisposable
{
    private readonly NotificationDbContext _entityNameDbContext;
    public NotificationPostgresRepository(NotificationDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<Notification>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Notification> entityNames = ImmutableList.CreateRange(_entityNameDbContext.Notifications.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<Notification> GetAsync(NotificationId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.Notifications
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(Notification entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.Notifications.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(NotificationId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.Notifications.Any(c => c.Id == id.Value));
    public async Task UpdateAsync(Notification entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.Notifications.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Notification entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(NotificationId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.Notifications.Single(c => c.Id == id.Value), cancellationToken);
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