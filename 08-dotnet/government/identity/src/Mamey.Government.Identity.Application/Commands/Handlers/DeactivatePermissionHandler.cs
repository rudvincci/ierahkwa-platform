using Mamey.CQRS.Commands;
using Mamey.Government.Identity.Application.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Exceptions;
using Mamey.Government.Identity.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Application.Commands.Handlers;

internal sealed class DeactivatePermissionHandler : ICommandHandler<DeactivatePermission>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly ILogger<DeactivatePermissionHandler> _logger;

    public DeactivatePermissionHandler(
        IPermissionRepository permissionRepository,
        ILogger<DeactivatePermissionHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _logger = logger;
    }

    public async Task HandleAsync(DeactivatePermission command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deactivating permission: {PermissionId}", command.Id);

        var permission = await _permissionRepository.GetAsync(command.Id, cancellationToken);
        if (permission is null)
        {
            throw new PermissionNotFoundException(command.Id);
        }

        permission.Deactivate();
        await _permissionRepository.UpdateAsync(permission, cancellationToken);

        _logger.LogInformation("Permission deactivated: {PermissionId}", command.Id);
    }
}
