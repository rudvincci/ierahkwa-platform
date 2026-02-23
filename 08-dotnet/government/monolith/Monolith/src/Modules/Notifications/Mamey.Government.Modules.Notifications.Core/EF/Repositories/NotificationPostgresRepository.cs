using Mamey.Government.Modules.Notifications.Core.Domain.Entities;
using Mamey.Government.Modules.Notifications.Core.Domain.Repositories;
using Mamey.Government.Modules.Notifications.Core.Domain.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Notifications.Core.EF.Repositories;

internal class NotificationPostgresRepository : INotificationRepository
{
    private readonly NotificationsDbContext _context;
    private readonly ILogger<NotificationPostgresRepository> _logger;

    public NotificationPostgresRepository(
        NotificationsDbContext context,
        ILogger<NotificationPostgresRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Notification?> GetAsync(NotificationId id, CancellationToken cancellationToken = default)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id.Value, cancellationToken);
        return notification;
    }

    public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await _context.Notifications.AddAsync(notification, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        _context.Notifications.Update(notification);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(NotificationId id, CancellationToken cancellationToken = default)
    {
        var notification = await GetAsync(id, cancellationToken);
        if (notification != null)
        {
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(NotificationId id, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .AnyAsync(n => n.Id == id.Value, cancellationToken);
    }

    public async Task<IReadOnlyList<Notification>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var notifications = await _context.Notifications.ToListAsync(cancellationToken);
        return notifications;
    }
}
