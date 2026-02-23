using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Compliance.Application.Events;

[Contract]
internal record ComplianceRecordDeleted(Guid ComplianceRecordId) : IEvent;


