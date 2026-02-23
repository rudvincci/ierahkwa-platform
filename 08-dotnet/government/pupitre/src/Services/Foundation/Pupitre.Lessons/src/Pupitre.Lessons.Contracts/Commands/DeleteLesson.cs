using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Lessons.Contracts.Commands;

[Contract]
public record DeleteLesson(Guid Id) : ICommand;


