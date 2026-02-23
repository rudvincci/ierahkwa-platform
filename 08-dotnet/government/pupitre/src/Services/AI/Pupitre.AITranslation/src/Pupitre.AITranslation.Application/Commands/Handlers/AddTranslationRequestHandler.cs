using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.AITranslation.Application.Exceptions;
using Pupitre.AITranslation.Contracts.Commands;
using Pupitre.AITranslation.Domain.Entities;
using Pupitre.AITranslation.Domain.Repositories;

namespace Pupitre.AITranslation.Application.Commands.Handlers;

internal sealed class AddTranslationRequestHandler : ICommandHandler<AddTranslationRequest>
{
    private readonly ITranslationRequestRepository _translationrequestRepository;
    private readonly IEventProcessor _eventProcessor;

    public AddTranslationRequestHandler(ITranslationRequestRepository translationrequestRepository,
        IEventProcessor eventProcessor)
    {
        _translationrequestRepository = translationrequestRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(AddTranslationRequest command, CancellationToken cancellationToken = default)
    {
        
        var translationrequest = await _translationrequestRepository.GetAsync(command.Id);
        
        if(translationrequest is not null)
        {
            throw new TranslationRequestAlreadyExistsException(command.Id);
        }

        translationrequest = TranslationRequest.Create(command.Id, command.Name ?? string.Empty, tags: command.Tags);
        await _translationrequestRepository.AddAsync(translationrequest, cancellationToken);
        await _eventProcessor.ProcessAsync(translationrequest.Events);
    }
}

