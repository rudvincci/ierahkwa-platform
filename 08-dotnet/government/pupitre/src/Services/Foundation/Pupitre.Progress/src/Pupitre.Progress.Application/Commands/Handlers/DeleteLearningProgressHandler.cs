using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Progress.Application.Exceptions;
using Pupitre.Progress.Contracts.Commands;
using Pupitre.Progress.Domain.Repositories;

namespace Pupitre.Progress.Application.Commands.Handlers;

internal sealed class DeleteLearningProgressHandler : ICommandHandler<DeleteLearningProgress>
{
    private readonly ILearningProgressRepository _learningprogressRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteLearningProgressHandler(ILearningProgressRepository learningprogressRepository, 
    IEventProcessor eventProcessor)
    {
        _learningprogressRepository = learningprogressRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteLearningProgress command, CancellationToken cancellationToken = default)
    {
        var learningprogress = await _learningprogressRepository.GetAsync(command.Id, cancellationToken);

        if (learningprogress is null)
        {
            throw new LearningProgressNotFoundException(command.Id);
        }

        await _learningprogressRepository.DeleteAsync(learningprogress.Id);
        await _eventProcessor.ProcessAsync(learningprogress.Events);
    }
}


