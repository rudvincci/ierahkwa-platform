using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.Government.Identity.Application.Events.Rejected;

[Contract]
internal record UpdateSubjectRejected(Guid SubjectId, string Reason, string Code) : IRejectedEvent;
