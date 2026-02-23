using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Application.Commands.Handlers;

internal sealed class CreatePermissionHandler : ICommandHandler<CreatePermission>
{
    private readonly IPermissionService _permissionService;
    private readonly IEventProcessor _eventProcessor;

    public CreatePermissionHandler(
        IPermissionService permissionService,
        IEventProcessor eventProcessor)
    {
        _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
        _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));
    }

    public async Task HandleAsync(CreatePermission command, CancellationToken cancellationToken = default)
    {
        var permissionId = await _permissionService.CreatePermissionAsync(
            command.Name,
            command.Description,
            cancellationToken);

        // Store permissionId in command context for response (if needed)
    }
}

