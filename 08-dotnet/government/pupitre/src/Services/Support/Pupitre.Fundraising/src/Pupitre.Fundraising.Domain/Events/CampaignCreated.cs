using Mamey.CQRS;
using Pupitre.Fundraising.Domain.Entities;

namespace Pupitre.Fundraising.Domain.Events;

internal record CampaignCreated(Campaign Campaign) : IDomainEvent;

