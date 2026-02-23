using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Rewards.Contracts.Commands;

[Contract]
public record DeleteReward(Guid Id) : ICommand;


