using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Commands.Handlers;

internal sealed class CreateRoleHandler : ICommandHandler<CreateRole>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IEventProcessor _eventProcessor;

    public CreateRoleHandler(IRoleRepository roleRepository, IEventProcessor eventProcessor)
    {
        _roleRepository = roleRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(CreateRole command, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetAsync(command.Id);
        
        if (role is not null)
        {
            throw new RoleAlreadyExistsException(command.Id);
        }

        var permissionIds = command.PermissionIds.Select(id => new PermissionId(id));
        role = Role.Create(command.Id, command.Name, command.Description, permissionIds);
        await _roleRepository.AddAsync(role, cancellationToken);
        await _eventProcessor.ProcessAsync(role.Events);
    }
}
