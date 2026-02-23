using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.AIAdaptive.Application.Exceptions;
using Pupitre.AIAdaptive.Contracts.Commands;
using Pupitre.AIAdaptive.Domain.Repositories;

namespace Pupitre.AIAdaptive.Application.Commands.Handlers;

internal sealed class DeleteAdaptiveLearningHandler : ICommandHandler<DeleteAdaptiveLearning>
{
    private readonly IAdaptiveLearningRepository _adaptivelearningRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteAdaptiveLearningHandler(IAdaptiveLearningRepository adaptivelearningRepository, 
    IEventProcessor eventProcessor)
    {
        _adaptivelearningRepository = adaptivelearningRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteAdaptiveLearning command, CancellationToken cancellationToken = default)
    {
        var adaptivelearning = await _adaptivelearningRepository.GetAsync(command.Id, cancellationToken);

        if (adaptivelearning is null)
        {
            throw new AdaptiveLearningNotFoundException(command.Id);
        }

        await _adaptivelearningRepository.DeleteAsync(adaptivelearning.Id);
        await _eventProcessor.ProcessAsync(adaptivelearning.Events);
    }
}


