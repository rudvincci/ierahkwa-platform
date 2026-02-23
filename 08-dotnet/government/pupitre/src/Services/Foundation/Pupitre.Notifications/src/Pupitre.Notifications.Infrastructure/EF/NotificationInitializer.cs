using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.Notifications.Infrastructure.EF;

internal sealed class NotificationInitializer
{
    private readonly NotificationDbContext _dbContext;
    private readonly ILogger<NotificationInitializer> _logger;

    public NotificationInitializer(NotificationDbContext dbContext, ILogger<NotificationInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.Notifications.AnyAsync())
        { }

        await AddNotificationsAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddNotificationsAsync()
    {
        throw new NotImplementedException();
    }
}