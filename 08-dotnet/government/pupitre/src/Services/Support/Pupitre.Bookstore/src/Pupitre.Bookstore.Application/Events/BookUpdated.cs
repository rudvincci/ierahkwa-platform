using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Bookstore.Application.Events;

[Contract]
internal record BookUpdated(Guid BookId) : IEvent;


