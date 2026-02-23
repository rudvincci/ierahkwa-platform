using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.ServiceName.Application.Exceptions;
using Mamey.ServiceName.Contracts.Commands;
using Mamey.ServiceName.Domain.Repositories;

namespace Mamey.ServiceName.Application.Commands.Handlers;

internal sealed class DeleteEntityNameHandler : ICommandHandler<DeleteEntityName>
{
    private readonly IEntityNameRepository _entitynameRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteEntityNameHandler(IEntityNameRepository entitynameRepository, 
    IEventProcessor eventProcessor)
    {
        _entitynameRepository = entitynameRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteEntityName command, CancellationToken cancellationToken = default)
    {
        var entityname = await _entitynameRepository.GetAsync(command.Id, cancellationToken);

        if (entityname is null)
        {
            throw new EntityNameNotFoundException(command.Id);
        }

        await _entitynameRepository.DeleteAsync(entityname.Id);
        await _eventProcessor.ProcessAsync(entityname.Events);
    }
}


