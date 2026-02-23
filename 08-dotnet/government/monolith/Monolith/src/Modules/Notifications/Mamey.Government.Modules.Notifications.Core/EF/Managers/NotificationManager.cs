using Mamey.Government.Modules.Notifications.Core.Domain.Entities;
using Mamey.Persistence.SQL;

namespace Mamey.Government.Modules.Notifications.Core.EF.Managers;

internal class NotificationManager : GenericManager<Notification>, INotificationManager
{
    
    public NotificationManager(NotificationsDbContext context) 
        : base(context)
    {
    }


    public IEnumerable<Notification> GetById(string id)
    {
        throw new NotImplementedException();
    }
}