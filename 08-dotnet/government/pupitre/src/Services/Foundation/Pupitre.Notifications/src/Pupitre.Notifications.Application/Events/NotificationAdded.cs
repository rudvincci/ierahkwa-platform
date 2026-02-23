using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Pupitre.Notifications.Application.Events;

[Contract]
internal record NotificationAdded(Guid NotificationId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

