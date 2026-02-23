using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.AIAdaptive.Application.Exceptions;
using Pupitre.AIAdaptive.Contracts.Commands;
using Pupitre.AIAdaptive.Domain.Entities;
using Pupitre.AIAdaptive.Domain.Repositories;

namespace Pupitre.AIAdaptive.Application.Commands.Handlers;

internal sealed class AddAdaptiveLearningHandler : ICommandHandler<AddAdaptiveLearning>
{
    private readonly IAdaptiveLearningRepository _adaptivelearningRepository;
    private readonly IEventProcessor _eventProcessor;

    public AddAdaptiveLearningHandler(IAdaptiveLearningRepository adaptivelearningRepository,
        IEventProcessor eventProcessor)
    {
        _adaptivelearningRepository = adaptivelearningRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(AddAdaptiveLearning command, CancellationToken cancellationToken = default)
    {
        
        var adaptivelearning = await _adaptivelearningRepository.GetAsync(command.Id);
        
        if(adaptivelearning is not null)
        {
            throw new AdaptiveLearningAlreadyExistsException(command.Id);
        }

        adaptivelearning = AdaptiveLearning.Create(command.Id, command.Name ?? string.Empty, tags: command.Tags);
        await _adaptivelearningRepository.AddAsync(adaptivelearning, cancellationToken);
        await _eventProcessor.ProcessAsync(adaptivelearning.Events);
    }
}

