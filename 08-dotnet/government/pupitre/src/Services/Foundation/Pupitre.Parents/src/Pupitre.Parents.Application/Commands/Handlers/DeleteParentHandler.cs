using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Parents.Application.Exceptions;
using Pupitre.Parents.Contracts.Commands;
using Pupitre.Parents.Domain.Repositories;

namespace Pupitre.Parents.Application.Commands.Handlers;

internal sealed class DeleteParentHandler : ICommandHandler<DeleteParent>
{
    private readonly IParentRepository _parentRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteParentHandler(IParentRepository parentRepository, 
    IEventProcessor eventProcessor)
    {
        _parentRepository = parentRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteParent command, CancellationToken cancellationToken = default)
    {
        var parent = await _parentRepository.GetAsync(command.Id, cancellationToken);

        if (parent is null)
        {
            throw new ParentNotFoundException(command.Id);
        }

        await _parentRepository.DeleteAsync(parent.Id);
        await _eventProcessor.ProcessAsync(parent.Events);
    }
}


