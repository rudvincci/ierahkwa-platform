using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Types;

namespace Mamey.FWID.Identities.Application.Commands.Handlers;

internal sealed class RemovePermissionFromRoleHandler : ICommandHandler<RemovePermissionFromRole>
{
    private readonly IRoleService _roleService;
    private readonly IEventProcessor _eventProcessor;

    public RemovePermissionFromRoleHandler(
        IRoleService roleService,
        IEventProcessor eventProcessor)
    {
        _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
        _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));
    }

    public async Task HandleAsync(RemovePermissionFromRole command, CancellationToken cancellationToken = default)
    {
        var roleId = new RoleId(command.RoleId);
        var permissionId = new PermissionId(command.PermissionId);
        await _roleService.RemovePermissionFromRoleAsync(
            roleId,
            permissionId,
            cancellationToken);
    }
}

