using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AIVision.Contracts.Commands;

[Contract]
public record DeleteVisionAnalysis(Guid Id) : ICommand;


