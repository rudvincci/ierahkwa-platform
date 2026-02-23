using Mamey.CQRS;
using Mamey.CQRS;
using Mamey.Government.Modules.Notifications.Core.Domain.Entities;
using Mamey.Government.Modules.Notifications.Core.Domain.Types;

namespace Mamey.Government.Modules.Notifications.Core.Domain.Events;

internal record NotificationModified(Notification Notification) : IDomainEvent;
