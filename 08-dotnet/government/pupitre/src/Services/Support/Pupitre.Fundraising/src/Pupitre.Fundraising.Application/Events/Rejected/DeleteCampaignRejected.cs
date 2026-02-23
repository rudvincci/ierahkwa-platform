using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Fundraising.Application.Events.Rejected;

[Contract]
internal record DeleteCampaignRejected(Guid CampaignId, string Reason, string Code) : IRejectedEvent;