using Mamey.CQRS;
using Pupitre.Rewards.Domain.Entities;

namespace Pupitre.Rewards.Domain.Events;

internal record RewardModified(Reward Reward): IDomainEvent;

