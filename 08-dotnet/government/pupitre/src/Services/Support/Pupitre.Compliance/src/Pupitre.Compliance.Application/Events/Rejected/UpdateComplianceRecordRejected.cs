using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Compliance.Application.Events.Rejected;

[Contract]
internal record UpdateComplianceRecordRejected(Guid ComplianceRecordId, string Reason, string Code) : IRejectedEvent;
