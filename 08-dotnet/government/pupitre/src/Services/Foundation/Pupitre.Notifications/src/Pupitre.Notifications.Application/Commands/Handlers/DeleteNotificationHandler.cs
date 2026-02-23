using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Notifications.Application.Exceptions;
using Pupitre.Notifications.Contracts.Commands;
using Pupitre.Notifications.Domain.Repositories;

namespace Pupitre.Notifications.Application.Commands.Handlers;

internal sealed class DeleteNotificationHandler : ICommandHandler<DeleteNotification>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteNotificationHandler(INotificationRepository notificationRepository, 
    IEventProcessor eventProcessor)
    {
        _notificationRepository = notificationRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteNotification command, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetAsync(command.Id, cancellationToken);

        if (notification is null)
        {
            throw new NotificationNotFoundException(command.Id);
        }

        await _notificationRepository.DeleteAsync(notification.Id);
        await _eventProcessor.ProcessAsync(notification.Events);
    }
}


