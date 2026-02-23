using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.AISpeech.Application.Exceptions;
using Pupitre.AISpeech.Contracts.Commands;
using Pupitre.AISpeech.Domain.Repositories;

namespace Pupitre.AISpeech.Application.Commands.Handlers;

internal sealed class DeleteSpeechRequestHandler : ICommandHandler<DeleteSpeechRequest>
{
    private readonly ISpeechRequestRepository _speechrequestRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteSpeechRequestHandler(ISpeechRequestRepository speechrequestRepository, 
    IEventProcessor eventProcessor)
    {
        _speechrequestRepository = speechrequestRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteSpeechRequest command, CancellationToken cancellationToken = default)
    {
        var speechrequest = await _speechrequestRepository.GetAsync(command.Id, cancellationToken);

        if (speechrequest is null)
        {
            throw new SpeechRequestNotFoundException(command.Id);
        }

        await _speechrequestRepository.DeleteAsync(speechrequest.Id);
        await _eventProcessor.ProcessAsync(speechrequest.Events);
    }
}


