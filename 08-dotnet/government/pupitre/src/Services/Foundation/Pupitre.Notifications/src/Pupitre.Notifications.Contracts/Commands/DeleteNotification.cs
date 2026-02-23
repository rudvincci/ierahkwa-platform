using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Notifications.Contracts.Commands;

[Contract]
public record DeleteNotification(Guid Id) : ICommand;


