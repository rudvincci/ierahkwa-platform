using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Commands.Handlers;

internal sealed class UpdatePermissionHandler : ICommandHandler<UpdatePermission>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IEventProcessor _eventProcessor;

    public UpdatePermissionHandler(IPermissionRepository permissionRepository, IEventProcessor eventProcessor)
    {
        _permissionRepository = permissionRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(UpdatePermission command, CancellationToken cancellationToken = default)
    {
        var permission = await _permissionRepository.GetAsync(command.Id);
        
        if (permission is null)
        {
            throw new PermissionNotFoundException(command.Id);
        }

        permission.Update(command.Name, command.Description, command.Resource, command.Action);
        await _permissionRepository.UpdateAsync(permission, cancellationToken);
        await _eventProcessor.ProcessAsync(permission.Events);
    }
}
