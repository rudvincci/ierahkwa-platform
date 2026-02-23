using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Ministries.Application.Events;

[Contract]
internal record MinistryDataDeleted(Guid MinistryDataId) : IEvent;


