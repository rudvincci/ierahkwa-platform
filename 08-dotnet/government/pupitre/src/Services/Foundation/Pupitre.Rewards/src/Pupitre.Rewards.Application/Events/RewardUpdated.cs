using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Rewards.Application.Events;

[Contract]
internal record RewardUpdated(Guid RewardId) : IEvent;


