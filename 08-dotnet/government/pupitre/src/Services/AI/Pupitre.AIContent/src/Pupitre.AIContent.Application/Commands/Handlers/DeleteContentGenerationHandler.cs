using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.AIContent.Application.Exceptions;
using Pupitre.AIContent.Contracts.Commands;
using Pupitre.AIContent.Domain.Repositories;

namespace Pupitre.AIContent.Application.Commands.Handlers;

internal sealed class DeleteContentGenerationHandler : ICommandHandler<DeleteContentGeneration>
{
    private readonly IContentGenerationRepository _contentgenerationRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteContentGenerationHandler(IContentGenerationRepository contentgenerationRepository, 
    IEventProcessor eventProcessor)
    {
        _contentgenerationRepository = contentgenerationRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteContentGeneration command, CancellationToken cancellationToken = default)
    {
        var contentgeneration = await _contentgenerationRepository.GetAsync(command.Id, cancellationToken);

        if (contentgeneration is null)
        {
            throw new ContentGenerationNotFoundException(command.Id);
        }

        await _contentgenerationRepository.DeleteAsync(contentgeneration.Id);
        await _eventProcessor.ProcessAsync(contentgeneration.Events);
    }
}


