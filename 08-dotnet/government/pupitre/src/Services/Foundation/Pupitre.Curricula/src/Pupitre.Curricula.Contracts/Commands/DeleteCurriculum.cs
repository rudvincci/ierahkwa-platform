using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Curricula.Contracts.Commands;

[Contract]
public record DeleteCurriculum(Guid Id) : ICommand;


