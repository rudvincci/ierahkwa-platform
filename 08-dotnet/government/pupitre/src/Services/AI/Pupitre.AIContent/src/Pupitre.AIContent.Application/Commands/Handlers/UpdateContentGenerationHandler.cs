using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.AIContent.Application.Exceptions;
using Pupitre.AIContent.Contracts.Commands;
using Pupitre.AIContent.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pupitre.AIContent.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateContentGenerationHandler : ICommandHandler<UpdateContentGeneration>
{
    private readonly IContentGenerationRepository _contentgenerationRepository;
    private readonly IEventProcessor _eventProcessor;

    public UpdateContentGenerationHandler(
        IContentGenerationRepository contentgenerationRepository,
        IEventProcessor eventProcessor)
    {
        _contentgenerationRepository = contentgenerationRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(UpdateContentGeneration command, CancellationToken cancellationToken = default)
    {
        var contentgeneration = await _contentgenerationRepository.GetAsync(command.Id);

        if(contentgeneration is null)
        {
            throw new ContentGenerationNotFoundException(command.Id);
        }

        contentgeneration.Update(command.Name, command.Tags);
        await _contentgenerationRepository.UpdateAsync(contentgeneration);
        await _eventProcessor.ProcessAsync(contentgeneration.Events);
    }
}


