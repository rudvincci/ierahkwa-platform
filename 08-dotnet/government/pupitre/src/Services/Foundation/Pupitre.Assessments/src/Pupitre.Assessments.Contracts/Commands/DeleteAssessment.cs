using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Assessments.Contracts.Commands;

[Contract]
public record DeleteAssessment(Guid Id) : ICommand;


