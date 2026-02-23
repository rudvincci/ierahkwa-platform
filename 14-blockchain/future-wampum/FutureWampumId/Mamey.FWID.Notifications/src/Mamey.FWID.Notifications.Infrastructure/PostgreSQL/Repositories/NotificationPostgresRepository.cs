using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Notifications.Domain.Entities;
using Mamey.FWID.Notifications.Domain.Repositories;
using Mamey.Persistence.SQL.Repositories;
using Mamey.Security;
using Microsoft.EntityFrameworkCore;

namespace Mamey.FWID.Notifications.Infrastructure.PostgreSQL.Repositories;

/// <summary>
/// PostgreSQL repository implementation for Notification entities.
/// This is the source of truth (write model) for notifications.
/// </summary>
internal class NotificationPostgresRepository : INotificationRepository, IDisposable
{
    private readonly NotificationDbContext _dbContext;
    private readonly SecurityAttributeProcessor _securityProcessor;

    public NotificationPostgresRepository(NotificationDbContext dbContext, SecurityAttributeProcessor securityProcessor)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _securityProcessor = securityProcessor ?? throw new ArgumentNullException(nameof(securityProcessor));
    }

    public async Task<Notification?> GetAsync(NotificationId id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Notifications
            .Where(n => n.Id.Value == id.Value)
            .SingleOrDefaultAsync(cancellationToken);

        if (entity != null)
        {
            // Reconstruct NotificationId with correct IdentityId
            var notificationId = new NotificationId(entity.Id.Value, entity.IdentityId);
            // Note: EF Core will handle the entity's Id property, but we need to ensure it matches
            
            // Process security attributes after loading (decrypt/hash verification)
            ProcessSecurityAttributesFromStorage(entity);
        }

        return entity;
    }

    public async Task<IEnumerable<Notification>> GetByIdentityIdAsync(IdentityId identityId, CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.Notifications
            .Where(n => n.IdentityId == identityId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);

        // Process security attributes after loading (decrypt/hash verification)
        foreach (var entity in entities)
        {
            ProcessSecurityAttributesFromStorage(entity);
        }

        return entities;
    }

    public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        // Process security attributes before saving (encrypt/hash)
        ProcessSecurityAttributesToStorage(notification);
        await _dbContext.Notifications.AddAsync(notification, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        // Process security attributes before saving (encrypt/hash)
        ProcessSecurityAttributesToStorage(notification);
        _dbContext.Notifications.Update(notification);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(NotificationId id, CancellationToken cancellationToken = default)
    {
        var entity = await GetAsync(id, cancellationToken);
        if (entity != null)
        {
            _dbContext.Notifications.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private void ProcessSecurityAttributesToStorage(Notification notification)
    {
        _securityProcessor.ProcessAllSecurityAttributes(notification, ProcessingDirection.ToStorage);
    }

    private void ProcessSecurityAttributesFromStorage(Notification notification)
    {
        _securityProcessor.ProcessAllSecurityAttributes(notification, ProcessingDirection.FromStorage);
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
    }
}

