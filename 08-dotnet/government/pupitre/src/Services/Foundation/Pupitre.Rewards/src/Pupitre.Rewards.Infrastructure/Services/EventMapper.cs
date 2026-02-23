using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.Rewards.Application.Events;
using Pupitre.Rewards.Domain.Events;

namespace Pupitre.Rewards.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // Reward
            RewardCreated e => null, // Event published thru handler
            RewardModified e => new RewardUpdated(e.Reward.Id),
            RewardRemoved e => new RewardDeleted(e.Reward.Id),
            RewardBlockchainRegistered e => new RewardCredentialIssued(e.RewardId, e.IdentityId, e.LedgerTransactionId),
            _ => null
        };
}

