using Mamey.CQRS;
using Pupitre.Rewards.Domain.Entities;

namespace Pupitre.Rewards.Domain.Events;

internal record RewardRemoved(Reward Reward) : IDomainEvent;