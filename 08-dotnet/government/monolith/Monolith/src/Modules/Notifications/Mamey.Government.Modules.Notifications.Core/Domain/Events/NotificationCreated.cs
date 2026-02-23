using Mamey.CQRS;
using Mamey.CQRS;
using Mamey.Government.Modules.Notifications.Core.Domain.Types;

namespace Mamey.Government.Modules.Notifications.Core.Domain.Events;

public record NotificationCreated(NotificationId NotificationId) : IDomainEvent;
