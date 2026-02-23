using Grpc.Core;
using Mamey.CQRS.Commands;
using Mamey.FWID.Identities.Api.Protos;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Api.Infrastructure.Grpc;

/// <summary>
/// gRPC service for permission synchronization.
/// </summary>
internal class PermissionSyncGrpcService : PermissionSyncService.PermissionSyncServiceBase
{
    private readonly IPermissionMappingRepository _repository;
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly ILogger<PermissionSyncGrpcService> _logger;

    public PermissionSyncGrpcService(
        IPermissionMappingRepository repository,
        ICommandDispatcher commandDispatcher,
        ILogger<PermissionSyncGrpcService> logger)
    {
        _repository = repository;
        _commandDispatcher = commandDispatcher;
        _logger = logger;
    }

    public override async Task<SyncPermissionsResponse> SyncPermissions(
        SyncPermissionsRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation(
            "Received gRPC SyncPermissions request for service: {ServiceName} with permissions: {Permissions}",
            request.ServiceName, string.Join(", ", request.Permissions));

        try
        {
            var command = new SyncPermissions
            {
                ServiceName = request.ServiceName,
                Permissions = request.Permissions.ToList(),
                CertificateSubject = request.CertificateSubject,
                CertificateIssuer = request.CertificateIssuer,
                CertificateThumbprint = request.CertificateThumbprint
            };

            await _commandDispatcher.SendAsync(command, context.CancellationToken);

            return new SyncPermissionsResponse
            {
                Success = true,
                ServiceName = request.ServiceName,
                Permissions = { request.Permissions },
                Message = "Permissions synchronized successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error synchronizing permissions for service: {ServiceName}", request.ServiceName);
            return new SyncPermissionsResponse
            {
                Success = false,
                ServiceName = request.ServiceName,
                Message = ex.Message
            };
        }
    }

    public override async Task<GetPermissionsResponse> GetPermissions(
        GetPermissionsRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("Received gRPC GetPermissions request for service: {ServiceName}", request.ServiceName);

        try
        {
            var mapping = await _repository.GetByServiceNameAsync(request.ServiceName, context.CancellationToken);

            if (mapping == null)
            {
                return new GetPermissionsResponse
                {
                    Success = false,
                    ServiceName = request.ServiceName,
                    Message = $"Permission mapping not found for service: {request.ServiceName}"
                };
            }

            return new GetPermissionsResponse
            {
                Success = true,
                ServiceName = mapping.ServiceName,
                Permissions = { mapping.Permissions },
                CertificateSubject = mapping.CertificateSubject ?? string.Empty,
                CertificateIssuer = mapping.CertificateIssuer ?? string.Empty,
                CertificateThumbprint = mapping.CertificateThumbprint ?? string.Empty,
                Message = "Permissions retrieved successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permissions for service: {ServiceName}", request.ServiceName);
            return new GetPermissionsResponse
            {
                Success = false,
                ServiceName = request.ServiceName,
                Message = ex.Message
            };
        }
    }

    public override async Task<UpdatePermissionsResponse> UpdatePermissions(
        UpdatePermissionsRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation(
            "Received gRPC UpdatePermissions request for service: {ServiceName} with permissions: {Permissions}",
            request.ServiceName, string.Join(", ", request.Permissions));

        try
        {
            var command = new SyncPermissions
            {
                ServiceName = request.ServiceName,
                Permissions = request.Permissions.ToList(),
                CertificateSubject = request.CertificateSubject,
                CertificateIssuer = request.CertificateIssuer,
                CertificateThumbprint = request.CertificateThumbprint
            };

            await _commandDispatcher.SendAsync(command, context.CancellationToken);

            return new UpdatePermissionsResponse
            {
                Success = true,
                ServiceName = request.ServiceName,
                Permissions = { request.Permissions },
                Message = "Permissions updated successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating permissions for service: {ServiceName}", request.ServiceName);
            return new UpdatePermissionsResponse
            {
                Success = false,
                ServiceName = request.ServiceName,
                Message = ex.Message
            };
        }
    }
}

