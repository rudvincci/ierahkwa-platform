using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.AISpeech.Application.Exceptions;
using Pupitre.AISpeech.Contracts.Commands;
using Pupitre.AISpeech.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pupitre.AISpeech.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateSpeechRequestHandler : ICommandHandler<UpdateSpeechRequest>
{
    private readonly ISpeechRequestRepository _speechrequestRepository;
    private readonly IEventProcessor _eventProcessor;

    public UpdateSpeechRequestHandler(
        ISpeechRequestRepository speechrequestRepository,
        IEventProcessor eventProcessor)
    {
        _speechrequestRepository = speechrequestRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(UpdateSpeechRequest command, CancellationToken cancellationToken = default)
    {
        var speechrequest = await _speechrequestRepository.GetAsync(command.Id);

        if(speechrequest is null)
        {
            throw new SpeechRequestNotFoundException(command.Id);
        }

        speechrequest.Update(command.Name, command.Tags);
        await _speechrequestRepository.UpdateAsync(speechrequest);
        await _eventProcessor.ProcessAsync(speechrequest.Events);
    }
}


