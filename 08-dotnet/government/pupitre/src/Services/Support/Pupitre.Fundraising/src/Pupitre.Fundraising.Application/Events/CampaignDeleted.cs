using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Fundraising.Application.Events;

[Contract]
internal record CampaignDeleted(Guid CampaignId) : IEvent;


