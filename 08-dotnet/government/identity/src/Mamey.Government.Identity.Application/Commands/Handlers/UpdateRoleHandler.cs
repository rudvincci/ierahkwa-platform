using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Commands.Handlers;

internal sealed class UpdateRoleHandler : ICommandHandler<UpdateRole>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IEventProcessor _eventProcessor;

    public UpdateRoleHandler(IRoleRepository roleRepository, IEventProcessor eventProcessor)
    {
        _roleRepository = roleRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(UpdateRole command, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetAsync(command.Id);
        
        if (role is null)
        {
            throw new RoleNotFoundException(command.Id);
        }

        var permissionIds = command.PermissionIds.Select(id => new PermissionId(id));
        role.Update(command.Name, command.Description, permissionIds);
        await _roleRepository.UpdateAsync(role, cancellationToken);
        await _eventProcessor.ProcessAsync(role.Events);
    }
}
