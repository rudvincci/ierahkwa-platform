using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AISpeech.Contracts.Commands;

[Contract]
public record DeleteSpeechRequest(Guid Id) : ICommand;


