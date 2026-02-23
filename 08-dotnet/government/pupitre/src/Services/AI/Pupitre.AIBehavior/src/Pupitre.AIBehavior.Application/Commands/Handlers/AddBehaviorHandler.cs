using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.AIBehavior.Application.Exceptions;
using Pupitre.AIBehavior.Contracts.Commands;
using Pupitre.AIBehavior.Domain.Entities;
using Pupitre.AIBehavior.Domain.Repositories;

namespace Pupitre.AIBehavior.Application.Commands.Handlers;

internal sealed class AddBehaviorHandler : ICommandHandler<AddBehavior>
{
    private readonly IBehaviorRepository _behaviorRepository;
    private readonly IEventProcessor _eventProcessor;

    public AddBehaviorHandler(IBehaviorRepository behaviorRepository,
        IEventProcessor eventProcessor)
    {
        _behaviorRepository = behaviorRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(AddBehavior command, CancellationToken cancellationToken = default)
    {
        
        var behavior = await _behaviorRepository.GetAsync(command.Id);
        
        if(behavior is not null)
        {
            throw new BehaviorAlreadyExistsException(command.Id);
        }

        behavior = Behavior.Create(command.Id, command.Name ?? string.Empty, tags: command.Tags);
        await _behaviorRepository.AddAsync(behavior, cancellationToken);
        await _eventProcessor.ProcessAsync(behavior.Events);
    }
}

