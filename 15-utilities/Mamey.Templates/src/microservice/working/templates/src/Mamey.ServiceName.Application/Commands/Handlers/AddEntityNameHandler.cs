using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.ServiceName.Application.Exceptions;
using Mamey.ServiceName.Contracts.Commands;
using Mamey.ServiceName.Domain.Entities;
using Mamey.ServiceName.Domain.Repositories;

namespace Mamey.ServiceName.Application.Commands.Handlers;

internal sealed class AddEntityNameHandler : ICommandHandler<AddEntityName>
{
    private readonly IEntityNameRepository _entitynameRepository;
    private readonly IEventProcessor _eventProcessor;

    public AddEntityNameHandler(IEntityNameRepository entitynameRepository,
        IEventProcessor eventProcessor)
    {
        _entitynameRepository = entitynameRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(AddEntityName command, CancellationToken cancellationToken = default)
    {
        
        var entityname = await _entitynameRepository.GetAsync(command.Id);
        
        if(entityname is not null)
        {
            throw new EntityNameAlreadyExistsException(command.Id);
        }

        entityname = EntityName.Create(command.Id, command.Name, tags: command.Tags);
        await _entitynameRepository.AddAsync(entityname, cancellationToken);
        await _eventProcessor.ProcessAsync(entityname.Events);
    }
}

