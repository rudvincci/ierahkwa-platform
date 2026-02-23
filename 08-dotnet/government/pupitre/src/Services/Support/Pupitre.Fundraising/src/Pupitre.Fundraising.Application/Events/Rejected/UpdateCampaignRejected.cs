using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Fundraising.Application.Events.Rejected;

[Contract]
internal record UpdateCampaignRejected(Guid CampaignId, string Reason, string Code) : IRejectedEvent;
