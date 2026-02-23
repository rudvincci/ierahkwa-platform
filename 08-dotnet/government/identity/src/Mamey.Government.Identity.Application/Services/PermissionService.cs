using System.Linq;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Application.Commands;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Events;
using Mamey.Government.Identity.Domain.Exceptions;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Time;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Application.Services;

internal sealed class PermissionService : IPermissionService
{
    #region Read-only Fields

    private readonly ILogger<PermissionService> _logger;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IClock _clock;
    private readonly IMessageBroker _messageBroker;
    private readonly IEventProcessor _eventProcessor;
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    #endregion

    public PermissionService(
        IPermissionRepository permissionRepository,
        IRoleRepository roleRepository,
        IClock clock,
        IMessageBroker messageBroker,
        IEventProcessor eventProcessor,
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        ILogger<PermissionService> logger)
    {
        _permissionRepository = permissionRepository;
        _roleRepository = roleRepository;
        _clock = clock;
        _messageBroker = messageBroker;
        _eventProcessor = eventProcessor;
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
        _logger = logger;
    }

    #region Permission CRUD Operations

    public async Task<PermissionDto?> GetPermissionAsync(PermissionId id, CancellationToken cancellationToken = default)
    {
        var query = new GetPermission(id);
        return await _queryDispatcher.QueryAsync<GetPermission, PermissionDto>(query, cancellationToken);
    }

    public async Task<PermissionDto> CreatePermissionAsync(CreatePermission command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating permission: {Name}", command.Name);

        // Check if permission already exists with same resource and action
        var existingPermission = await _permissionRepository.GetByResourceAndActionAsync(command.Resource, command.Action, cancellationToken);
        if (existingPermission is not null)
        {
            throw new Mamey.Government.Identity.Domain.Exceptions.PermissionAlreadyExistsException(command.Resource, command.Action);
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);
        
        var permission = await _permissionRepository.GetAsync(command.Id, cancellationToken);
        if (permission is null)
        {
            throw new Mamey.Government.Identity.Domain.Exceptions.PermissionNotFoundException(command.Id);
        }

        await _messageBroker.PublishAsync(new Mamey.Government.Identity.Application.Events.PermissionCreated(command.Id, command.Name, command.Resource, command.Action, _clock.CurrentDate()));

        return MapToPermissionDto(permission);
    }

    public async Task<PermissionDto> UpdatePermissionAsync(UpdatePermission command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating permission: {PermissionId}", command.Id);

        var permission = await _permissionRepository.GetAsync(command.Id, cancellationToken);
        if (permission is null)
        {
            throw new Mamey.Government.Identity.Domain.Exceptions.PermissionNotFoundException(command.Id);
        }

        // Check if another permission exists with same resource and action
        var existingPermissions = await _permissionRepository.GetByResourceAndActionAsync(command.Resource, command.Action, cancellationToken);
        var existingPermission = existingPermissions.FirstOrDefault();
        if (existingPermission is not null && existingPermission.Id != command.Id)
        {
            throw new Mamey.Government.Identity.Domain.Exceptions.PermissionAlreadyExistsException(command.Resource, command.Action);
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);
        
        permission = await _permissionRepository.GetAsync(command.Id, cancellationToken);
        if (permission is null)
        {
            throw new Mamey.Government.Identity.Domain.Exceptions.PermissionNotFoundException(command.Id);
        }

        return MapToPermissionDto(permission);
    }

    public async Task DeletePermissionAsync(PermissionId id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting permission: {PermissionId}", id);

        var permission = await _permissionRepository.GetAsync(id, cancellationToken);
        if (permission is null)
        {
            throw new Mamey.Government.Identity.Domain.Exceptions.PermissionNotFoundException(id);
        }

        // Check if permission is assigned to any roles
        var rolesWithPermission = await _roleRepository.GetByPermissionIdAsync(id, cancellationToken);
        if (rolesWithPermission.Any())
        {
            throw new Mamey.Government.Identity.Domain.Exceptions.PermissionInUseException(id);
        }

        // Soft delete by deactivating
        var deactivateCommand = new DeactivatePermission(id);
        await _commandDispatcher.SendAsync(deactivateCommand, cancellationToken);
    }

    #endregion

    #region Permission Search and Filtering

    public async Task<IEnumerable<PermissionDto>> GetPermissionsByStatusAsync(PermissionStatus status, CancellationToken cancellationToken = default)
    {
        var permissions = await _permissionRepository.GetByStatusAsync(status, cancellationToken);
        return permissions.Select(MapToPermissionDto);
    }

    public async Task<IEnumerable<PermissionDto>> GetPermissionsByResourceAsync(string resource, CancellationToken cancellationToken = default)
    {
        var permissions = await _permissionRepository.GetByResourceAsync(resource, cancellationToken);
        return permissions.Select(MapToPermissionDto);
    }

    public async Task<IEnumerable<PermissionDto>> GetPermissionsByActionAsync(string action, CancellationToken cancellationToken = default)
    {
        var permissions = await _permissionRepository.GetByActionAsync(action, cancellationToken);
        return permissions.Select(MapToPermissionDto);
    }

    public async Task<IEnumerable<PermissionDto>> SearchPermissionsAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var permissions = await _permissionRepository.SearchAsync(searchTerm, cancellationToken);
        return permissions.Select(MapToPermissionDto);
    }

    public async Task<IEnumerable<PermissionDto>> GetPermissionsByRoleAsync(RoleId roleId, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetAsync(roleId, cancellationToken);
        if (role is null)
        {
            throw new Mamey.Government.Identity.Domain.Exceptions.RoleNotFoundException(roleId);
        }

        var permissions = await _permissionRepository.GetByIdsAsync(role.Permissions, cancellationToken);
        return permissions.Select(MapToPermissionDto);
    }

    #endregion

    #region Permission Management

    public async Task ActivatePermissionAsync(PermissionId id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Activating permission: {PermissionId}", id);

        var permission = await _permissionRepository.GetAsync(id, cancellationToken);
        if (permission is null)
        {
            throw new Mamey.Government.Identity.Domain.Exceptions.PermissionNotFoundException(id);
        }

        if (permission.Status == PermissionStatus.Active)
        {
            throw new PermissionAlreadyActiveException(id);
        }

        permission.Activate();
        await _permissionRepository.UpdateAsync(permission, cancellationToken);
        await _eventProcessor.ProcessAsync(permission.Events);
    }

    public async Task DeactivatePermissionAsync(PermissionId id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deactivating permission: {PermissionId}", id);

        var permission = await _permissionRepository.GetAsync(id, cancellationToken);
        if (permission is null)
        {
            throw new Mamey.Government.Identity.Domain.Exceptions.PermissionNotFoundException(id);
        }

        if (permission.Status == PermissionStatus.Inactive)
        {
            throw new PermissionAlreadyInactiveException(id);
        }

        permission.Deactivate();
        await _permissionRepository.UpdateAsync(permission, cancellationToken);
        await _eventProcessor.ProcessAsync(permission.Events);
    }

    #endregion

    #region Permission Statistics

    public async Task<PermissionStatisticsDto> GetPermissionStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var totalPermissions = await _permissionRepository.CountAsync(cancellationToken);
        var activePermissions = await _permissionRepository.CountByStatusAsync(PermissionStatus.Active, cancellationToken);
        var inactivePermissions = await _permissionRepository.CountByStatusAsync(PermissionStatus.Inactive, cancellationToken);

        // Get resource and action statistics
        var resources = await _permissionRepository.GetDistinctResourcesAsync(cancellationToken);
        var actions = await _permissionRepository.GetDistinctActionsAsync(cancellationToken);

        return new PermissionStatisticsDto
        {
            TotalPermissions = totalPermissions,
            ActivePermissions = activePermissions,
            InactivePermissions = inactivePermissions,
            UniqueResources = resources.Count(),
            UniqueActions = actions.Count()
        };
    }

    #endregion

    #region Private Helper Methods

    private static PermissionDto MapToPermissionDto(Permission permission)
    {
        return new PermissionDto(
            permission.Id,
            permission.Name,
            permission.Description,
            permission.Resource,
            permission.Action,
            permission.Status.ToString(),
            permission.CreatedAt,
            permission.ModifiedAt
        );
    }

    #endregion
}

