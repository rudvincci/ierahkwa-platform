using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Analytics.Contracts.Commands;

[Contract]
public record DeleteAnalytic(Guid Id) : ICommand;


