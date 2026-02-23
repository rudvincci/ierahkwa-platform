using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.AIContent.Application.Exceptions;
using Pupitre.AIContent.Contracts.Commands;
using Pupitre.AIContent.Domain.Entities;
using Pupitre.AIContent.Domain.Repositories;

namespace Pupitre.AIContent.Application.Commands.Handlers;

internal sealed class AddContentGenerationHandler : ICommandHandler<AddContentGeneration>
{
    private readonly IContentGenerationRepository _contentgenerationRepository;
    private readonly IEventProcessor _eventProcessor;

    public AddContentGenerationHandler(IContentGenerationRepository contentgenerationRepository,
        IEventProcessor eventProcessor)
    {
        _contentgenerationRepository = contentgenerationRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(AddContentGeneration command, CancellationToken cancellationToken = default)
    {
        
        var contentgeneration = await _contentgenerationRepository.GetAsync(command.Id);
        
        if(contentgeneration is not null)
        {
            throw new ContentGenerationAlreadyExistsException(command.Id);
        }

        contentgeneration = ContentGeneration.Create(command.Id, command.Name ?? string.Empty, tags: command.Tags);
        await _contentgenerationRepository.AddAsync(contentgeneration, cancellationToken);
        await _eventProcessor.ProcessAsync(contentgeneration.Events);
    }
}

