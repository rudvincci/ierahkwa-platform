using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Rewards.Application.Events.Rejected;

[Contract]
internal record AddRewardRejected(Guid RewardId, string Reason, string Code) : IRejectedEvent;
