using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.AIBehavior.Application.Exceptions;
using Pupitre.AIBehavior.Contracts.Commands;
using Pupitre.AIBehavior.Domain.Repositories;

namespace Pupitre.AIBehavior.Application.Commands.Handlers;

internal sealed class DeleteBehaviorHandler : ICommandHandler<DeleteBehavior>
{
    private readonly IBehaviorRepository _behaviorRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteBehaviorHandler(IBehaviorRepository behaviorRepository, 
    IEventProcessor eventProcessor)
    {
        _behaviorRepository = behaviorRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteBehavior command, CancellationToken cancellationToken = default)
    {
        var behavior = await _behaviorRepository.GetAsync(command.Id, cancellationToken);

        if (behavior is null)
        {
            throw new BehaviorNotFoundException(command.Id);
        }

        await _behaviorRepository.DeleteAsync(behavior.Id);
        await _eventProcessor.ProcessAsync(behavior.Events);
    }
}


