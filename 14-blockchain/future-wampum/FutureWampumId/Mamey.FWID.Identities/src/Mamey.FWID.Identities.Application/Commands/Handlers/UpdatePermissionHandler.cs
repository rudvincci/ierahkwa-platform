using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Application.Commands.Handlers;

internal sealed class UpdatePermissionHandler : ICommandHandler<UpdatePermission>
{
    private readonly IPermissionService _permissionService;
    private readonly IEventProcessor _eventProcessor;

    public UpdatePermissionHandler(
        IPermissionService permissionService,
        IEventProcessor eventProcessor)
    {
        _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
        _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));
    }

    public async Task HandleAsync(UpdatePermission command, CancellationToken cancellationToken = default)
    {
        var permissionId = new PermissionId(command.PermissionId);
        await _permissionService.UpdatePermissionAsync(
            permissionId,
            command.Name,
            command.Description,
            cancellationToken);
    }
}

