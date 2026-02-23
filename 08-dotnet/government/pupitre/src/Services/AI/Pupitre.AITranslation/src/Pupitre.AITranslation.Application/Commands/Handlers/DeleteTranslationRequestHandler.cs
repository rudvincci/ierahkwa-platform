using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.AITranslation.Application.Exceptions;
using Pupitre.AITranslation.Contracts.Commands;
using Pupitre.AITranslation.Domain.Repositories;

namespace Pupitre.AITranslation.Application.Commands.Handlers;

internal sealed class DeleteTranslationRequestHandler : ICommandHandler<DeleteTranslationRequest>
{
    private readonly ITranslationRequestRepository _translationrequestRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteTranslationRequestHandler(ITranslationRequestRepository translationrequestRepository, 
    IEventProcessor eventProcessor)
    {
        _translationrequestRepository = translationrequestRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteTranslationRequest command, CancellationToken cancellationToken = default)
    {
        var translationrequest = await _translationrequestRepository.GetAsync(command.Id, cancellationToken);

        if (translationrequest is null)
        {
            throw new TranslationRequestNotFoundException(command.Id);
        }

        await _translationrequestRepository.DeleteAsync(translationrequest.Id);
        await _eventProcessor.ProcessAsync(translationrequest.Events);
    }
}


