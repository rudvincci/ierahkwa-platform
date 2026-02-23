using Mamey.CQRS.Commands;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Commands.Handlers;

/// <summary>
/// Handler for synchronizing permissions from other services.
/// </summary>
internal sealed class SyncPermissionsHandler : ICommandHandler<SyncPermissions>
{
    private readonly IPermissionMappingRepository _repository;
    private readonly ILogger<SyncPermissionsHandler> _logger;

    public SyncPermissionsHandler(
        IPermissionMappingRepository repository,
        ILogger<SyncPermissionsHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task HandleAsync(SyncPermissions command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Synchronizing permissions for service: {ServiceName} with permissions: {Permissions}",
            command.ServiceName, string.Join(", ", command.Permissions));

        // Check if mapping already exists
        var existingMapping = await _repository.GetByServiceNameAsync(command.ServiceName, cancellationToken);
        
        if (existingMapping != null)
        {
            // Update existing mapping
            existingMapping.UpdatePermissions(command.Permissions);
            existingMapping.UpdateCertificateInfo(
                command.CertificateSubject,
                command.CertificateIssuer,
                command.CertificateThumbprint);
            
            await _repository.UpdateAsync(existingMapping, cancellationToken);
            
            _logger.LogInformation(
                "Updated permission mapping for service: {ServiceName}",
                command.ServiceName);
        }
        else
        {
            // Create new mapping
            var mapping = new PermissionMapping(
                Guid.NewGuid(),
                command.ServiceName,
                command.Permissions,
                command.CertificateSubject,
                command.CertificateIssuer,
                command.CertificateThumbprint);
            
            await _repository.AddAsync(mapping, cancellationToken);
            
            _logger.LogInformation(
                "Created new permission mapping for service: {ServiceName}",
                command.ServiceName);
        }
    }
}

