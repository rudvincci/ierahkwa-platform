using Mamey.Government.Modules.Notifications.Core.DTO;
using Mamey.CQRS.Queries;

namespace Mamey.Government.Modules.Notifications.Core.Queries;

internal class BrowseNotifications : PagedQueryBase, IQuery<PagedResult<NotificationDto>>
{
    public BrowseNotifications()
    {
    }
    public Guid UserId { get; set; }
    public bool ShowRead { get; set; }
}
internal class ListNotifications : IQuery<List<NotificationDetailsDto>>
{
    public ListNotifications()
    {
    }
}