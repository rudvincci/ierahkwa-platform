using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Types;

namespace Mamey.FWID.Identities.Application.Commands.Handlers;

internal sealed class AssignRoleToIdentityHandler : ICommandHandler<AssignRoleToIdentity>
{
    private readonly IRoleService _roleService;
    private readonly IEventProcessor _eventProcessor;

    public AssignRoleToIdentityHandler(
        IRoleService roleService,
        IEventProcessor eventProcessor)
    {
        _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
        _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));
    }

    public async Task HandleAsync(AssignRoleToIdentity command, CancellationToken cancellationToken = default)
    {
        var identityId = new IdentityId(command.IdentityId);
        var roleId = new RoleId(command.RoleId);
        await _roleService.AssignRoleToIdentityAsync(
            identityId,
            roleId,
            cancellationToken);
    }
}

