using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Notifications.Application.Commands;

/// <summary>
/// Command to add a notification.
/// </summary>
[Contract]
internal record AddNotification : ICommand
{
    public AddNotification(Guid identityId, string title, string description, string message, string notificationType)
    {
        IdentityId = identityId;
        Title = title;
        Description = description;
        Message = message;
        NotificationType = notificationType;
    }

    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid IdentityId { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public string Message { get; init; }
    public string NotificationType { get; init; }
}







