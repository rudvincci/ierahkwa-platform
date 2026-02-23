using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.AIBehavior.Application.Exceptions;
using Pupitre.AIBehavior.Contracts.Commands;
using Pupitre.AIBehavior.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pupitre.AIBehavior.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateBehaviorHandler : ICommandHandler<UpdateBehavior>
{
    private readonly IBehaviorRepository _behaviorRepository;
    private readonly IEventProcessor _eventProcessor;

    public UpdateBehaviorHandler(
        IBehaviorRepository behaviorRepository,
        IEventProcessor eventProcessor)
    {
        _behaviorRepository = behaviorRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(UpdateBehavior command, CancellationToken cancellationToken = default)
    {
        var behavior = await _behaviorRepository.GetAsync(command.Id);

        if(behavior is null)
        {
            throw new BehaviorNotFoundException(command.Id);
        }

        behavior.Update(command.Name, command.Tags);
        await _behaviorRepository.UpdateAsync(behavior);
        await _eventProcessor.ProcessAsync(behavior.Events);
    }
}


