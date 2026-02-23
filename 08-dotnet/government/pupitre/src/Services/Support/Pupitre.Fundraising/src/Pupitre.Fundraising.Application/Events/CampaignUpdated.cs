using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Fundraising.Application.Events;

[Contract]
internal record CampaignUpdated(Guid CampaignId) : IEvent;


