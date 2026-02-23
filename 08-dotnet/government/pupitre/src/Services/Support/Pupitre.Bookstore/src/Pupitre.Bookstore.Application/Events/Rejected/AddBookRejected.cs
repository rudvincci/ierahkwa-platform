using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Bookstore.Application.Events.Rejected;

[Contract]
internal record AddBookRejected(Guid BookId, string Reason, string Code) : IRejectedEvent;
