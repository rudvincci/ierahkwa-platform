using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Notifications.Domain.Entities;
using Pupitre.Notifications.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Notifications.Infrastructure.PostgreSQL.Repositories;

internal class NotificationPostgresRepository : EFRepository<Notification, NotificationId>, INotificationRepository, IDisposable
{
    private readonly NotificationsDbContext _serviceNameDbContext;
    public NotificationPostgresRepository(NotificationsDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<Notification>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Notification> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.Notifications.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<Notification> GetAsync(NotificationId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.Notifications
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(Notification entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.Notifications.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(NotificationId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.Notifications.Any(c => c.Id == id.Value));
    public async Task UpdateAsync(Notification entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.Notifications.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Notification entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(NotificationId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.Notifications.Single(c => c.Id == id.Value), cancellationToken);
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