using Mamey.CQRS.Commands;
using Mamey.Government.Identity.Application.Commands;
using Mamey.Government.Identity.Domain.Exceptions;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Application.Commands.Handlers;

internal sealed class DeactivateRoleHandler : ICommandHandler<DeactivateRole>
{
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<DeactivateRoleHandler> _logger;

    public DeactivateRoleHandler(
        IRoleRepository roleRepository,
        ILogger<DeactivateRoleHandler> logger)
    {
        _roleRepository = roleRepository;
        _logger = logger;
    }

    public async Task HandleAsync(DeactivateRole command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deactivating role: {RoleId}", command.Id);

        var role = await _roleRepository.GetAsync(command.Id, cancellationToken);
        if (role is null)
        {
            throw new RoleNotFoundException(command.Id);
        }

        role.Deactivate();
        await _roleRepository.UpdateAsync(role, cancellationToken);

        _logger.LogInformation("Role deactivated: {RoleId}", command.Id);
    }
}
