using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Ministries.Application.Events.Rejected;

[Contract]
internal record UpdateMinistryDataRejected(Guid MinistryDataId, string Reason, string Code) : IRejectedEvent;
