using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Accessibility.Contracts.Commands;

[Contract]
public record DeleteAccessProfile(Guid Id) : ICommand;


