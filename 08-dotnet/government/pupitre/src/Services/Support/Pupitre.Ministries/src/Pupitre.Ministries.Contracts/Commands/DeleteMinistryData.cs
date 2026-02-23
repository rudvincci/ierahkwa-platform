using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Ministries.Contracts.Commands;

[Contract]
public record DeleteMinistryData(Guid Id) : ICommand;


