using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.ServiceName.Application.Exceptions;
using Mamey.ServiceName.Contracts.Commands;
using Mamey.ServiceName.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Mamey.ServiceName.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateEntityNameHandler : ICommandHandler<UpdateEntityName>
{
    private readonly IEntityNameRepository _entitynameRepository;
    private readonly IEventProcessor _eventProcessor;

    public UpdateEntityNameHandler(
        IEntityNameRepository entitynameRepository,
        IEventProcessor eventProcessor)
    {
        _entitynameRepository = entitynameRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(UpdateEntityName command, CancellationToken cancellationToken = default)
    {
        var entityname = await _entitynameRepository.GetAsync(command.Id);

        if(entityname is null)
        {
            throw new EntityNameNotFoundException(command.Id);
        }

        entityname.Update(command.Name, command.Tags);
        await _entitynameRepository.UpdateAsync(entityname);
        await _eventProcessor.ProcessAsync(entityname.Events);
    }
}


