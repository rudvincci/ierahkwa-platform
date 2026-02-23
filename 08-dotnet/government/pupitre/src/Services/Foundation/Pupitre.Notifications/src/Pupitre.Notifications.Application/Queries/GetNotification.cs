using System;
using Mamey.CQRS.Queries;
using Pupitre.Notifications.Application.DTO;

namespace Pupitre.Notifications.Application.Queries;

internal record GetNotification(Guid Id) : IQuery<NotificationDetailsDto>;
