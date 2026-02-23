using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Compliance.Application.Events;

[Contract]
internal record ComplianceRecordUpdated(Guid ComplianceRecordId) : IEvent;


