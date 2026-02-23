using System;
using Mamey.CQRS.Queries;
using Pupitre.Notifications.Application.DTO;


namespace Pupitre.Notifications.Application.Queries;

internal class BrowseNotifications : PagedQueryBase, IQuery<PagedResult<NotificationDto>?>
{
    public string? Name { get; set; }
}