using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.AISpeech.Application.Exceptions;
using Pupitre.AISpeech.Contracts.Commands;
using Pupitre.AISpeech.Domain.Entities;
using Pupitre.AISpeech.Domain.Repositories;

namespace Pupitre.AISpeech.Application.Commands.Handlers;

internal sealed class AddSpeechRequestHandler : ICommandHandler<AddSpeechRequest>
{
    private readonly ISpeechRequestRepository _speechrequestRepository;
    private readonly IEventProcessor _eventProcessor;

    public AddSpeechRequestHandler(ISpeechRequestRepository speechrequestRepository,
        IEventProcessor eventProcessor)
    {
        _speechrequestRepository = speechrequestRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(AddSpeechRequest command, CancellationToken cancellationToken = default)
    {
        
        var speechrequest = await _speechrequestRepository.GetAsync(command.Id);
        
        if(speechrequest is not null)
        {
            throw new SpeechRequestAlreadyExistsException(command.Id);
        }

        speechrequest = SpeechRequest.Create(command.Id, command.Name ?? string.Empty, tags: command.Tags);
        await _speechrequestRepository.AddAsync(speechrequest, cancellationToken);
        await _eventProcessor.ProcessAsync(speechrequest.Events);
    }
}

