using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Application.Commands.Handlers;

internal sealed class CreateRoleHandler : ICommandHandler<CreateRole>
{
    private readonly IRoleService _roleService;
    private readonly IEventProcessor _eventProcessor;

    public CreateRoleHandler(
        IRoleService roleService,
        IEventProcessor eventProcessor)
    {
        _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
        _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));
    }

    public async Task HandleAsync(CreateRole command, CancellationToken cancellationToken = default)
    {
        var roleId = await _roleService.CreateRoleAsync(
            command.Name,
            command.Description,
            cancellationToken);

        // Store roleId in command context for response (if needed)
    }
}

