using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AITranslation.Contracts.Commands;

[Contract]
public record DeleteTranslationRequest(Guid Id) : ICommand;


