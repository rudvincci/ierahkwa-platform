using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.AITranslation.Application.Exceptions;
using Pupitre.AITranslation.Contracts.Commands;
using Pupitre.AITranslation.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pupitre.AITranslation.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateTranslationRequestHandler : ICommandHandler<UpdateTranslationRequest>
{
    private readonly ITranslationRequestRepository _translationrequestRepository;
    private readonly IEventProcessor _eventProcessor;

    public UpdateTranslationRequestHandler(
        ITranslationRequestRepository translationrequestRepository,
        IEventProcessor eventProcessor)
    {
        _translationrequestRepository = translationrequestRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(UpdateTranslationRequest command, CancellationToken cancellationToken = default)
    {
        var translationrequest = await _translationrequestRepository.GetAsync(command.Id);

        if(translationrequest is null)
        {
            throw new TranslationRequestNotFoundException(command.Id);
        }

        translationrequest.Update(command.Name, command.Tags);
        await _translationrequestRepository.UpdateAsync(translationrequest);
        await _eventProcessor.ProcessAsync(translationrequest.Events);
    }
}


