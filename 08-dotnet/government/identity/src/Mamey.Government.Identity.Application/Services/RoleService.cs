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
using PermissionNotFoundException = Mamey.Government.Identity.Application.Exceptions.PermissionNotFoundException;

namespace Mamey.Government.Identity.Application.Services;

internal sealed class RoleService : IRoleService
{
    #region Read-only Fields

    private readonly ILogger<RoleService> _logger;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IClock _clock;
    private readonly IMessageBroker _messageBroker;
    private readonly IEventProcessor _eventProcessor;
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    #endregion

    public RoleService(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        ISubjectRepository subjectRepository,
        IClock clock,
        IMessageBroker messageBroker,
        IEventProcessor eventProcessor,
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        ILogger<RoleService> logger)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _subjectRepository = subjectRepository;
        _clock = clock;
        _messageBroker = messageBroker;
        _eventProcessor = eventProcessor;
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
        _logger = logger;
    }

    #region Role CRUD Operations

    public async Task<RoleDto?> GetRoleAsync(RoleId id, CancellationToken cancellationToken = default)
    {
        var query = new GetRole(id);
        return await _queryDispatcher.QueryAsync<GetRole, RoleDto>(query, cancellationToken);
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRole command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating role: {Name}", command.Name);

        // Validate permissions exist
        if (command.PermissionIds.Any())
        {
            var permissions = await _permissionRepository.GetByIdsAsync(command.PermissionIds.Select(id => new PermissionId(id)), cancellationToken);
            if (permissions.Count() != command.PermissionIds.Count())
            {
                throw new InvalidRolePermissionsException();
            }
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);
        
        var role = await _roleRepository.GetAsync(command.Id, cancellationToken);
        if (role is null)
        {
            throw new Mamey.Government.Identity.Domain.Exceptions.RoleNotFoundException(command.Id);
        }

        await _messageBroker.PublishAsync(new Mamey.Government.Identity.Application.Events.RoleCreated(command.Id, command.Name, command.Description, _clock.CurrentDate()));

        return MapToRoleDto(role);
    }

    public async Task<RoleDto> UpdateRoleAsync(UpdateRole command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating role: {RoleId}", command.Id);

        var role = await _roleRepository.GetAsync(command.Id, cancellationToken);
        if (role is null)
        {
            throw new Mamey.Government.Identity.Domain.Exceptions.RoleNotFoundException(command.Id);
        }

        // Validate permissions exist
        if (command.PermissionIds.Any())
        {
            var permissions = await _permissionRepository.GetByIdsAsync(command.PermissionIds.Select(id => new PermissionId(id)), cancellationToken);
            if (permissions.Count() != command.PermissionIds.Count())
            {
                throw new InvalidRolePermissionsException();
            }
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);
        
        role = await _roleRepository.GetAsync(command.Id, cancellationToken);
        if (role is null)
        {
            throw new Mamey.Government.Identity.Domain.Exceptions.RoleNotFoundException(command.Id);
        }

        return MapToRoleDto(role);
    }

    public async Task DeleteRoleAsync(RoleId id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting role: {RoleId}", id);

        var role = await _roleRepository.GetAsync(id, cancellationToken);
        if (role is null)
        {
            throw new Mamey.Government.Identity.Domain.Exceptions.RoleNotFoundException(id);
        }

        // Check if role is assigned to any subjects
        var subjectsWithRole = await _subjectRepository.GetByRoleIdAsync(id, cancellationToken);
        if (subjectsWithRole.Any())
        {
            throw new RoleInUseException(id);
        }

        // Soft delete by deactivating
        var deactivateCommand = new DeactivateRole(id);
        await _commandDispatcher.SendAsync(deactivateCommand, cancellationToken);
    }

    #endregion

    #region Role Assignment Management

    public async Task AssignRoleToSubjectAsync(AssignRoleToSubject command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Assigning role {RoleId} to subject {SubjectId}", command.RoleId, command.SubjectId);

        var subject = await _subjectRepository.GetAsync(command.SubjectId, cancellationToken);
        if (subject is null)
        {
            throw new SubjectNotFoundException(command.SubjectId);
        }

        var role = await _roleRepository.GetAsync(command.RoleId, cancellationToken);
        if (role is null)
        {
            throw new Mamey.Government.Identity.Domain.Exceptions.RoleNotFoundException(command.RoleId);
        }

        if (role.Status != RoleStatus.Active)
        {
            throw new RoleNotActiveException(command.RoleId);
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);
    }

    public async Task RemoveRoleFromSubjectAsync(RemoveRoleFromSubject command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Removing role {RoleId} from subject {SubjectId}", command.RoleId, command.SubjectId);

        var subject = await _subjectRepository.GetAsync(command.SubjectId, cancellationToken);
        if (subject is null)
        {
            throw new SubjectNotFoundException(command.SubjectId);
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);
    }

    #endregion

    #region Role Search and Filtering

    public async Task<IEnumerable<RoleDto>> GetRolesByStatusAsync(RoleStatus status, CancellationToken cancellationToken = default)
    {
        var roles = await _roleRepository.GetByStatusAsync(status, cancellationToken);
        return roles.Select(MapToRoleDto);
    }

    public async Task<IEnumerable<RoleDto>> GetRolesByPermissionAsync(PermissionId permissionId, CancellationToken cancellationToken = default)
    {
        var roles = await _roleRepository.GetByPermissionIdAsync(permissionId, cancellationToken);
        return roles.Select(MapToRoleDto);
    }

    public async Task<IEnumerable<RoleDto>> SearchRolesAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var roles = await _roleRepository.SearchAsync(searchTerm, cancellationToken);
        return roles.Select(MapToRoleDto);
    }

    #endregion

    #region Role Statistics

    public async Task<RoleStatisticsDto> GetRoleStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var totalRoles = await _roleRepository.CountAsync(cancellationToken);
        var activeRoles = await _roleRepository.CountByStatusAsync(RoleStatus.Active, cancellationToken);
        var inactiveRoles = await _roleRepository.CountByStatusAsync(RoleStatus.Inactive, cancellationToken);
        var deprecatedRoles = await _roleRepository.CountByStatusAsync(RoleStatus.Deprecated, cancellationToken);

        return new RoleStatisticsDto
        {
            TotalRoles = totalRoles,
            ActiveRoles = activeRoles,
            InactiveRoles = inactiveRoles,
            DeprecatedRoles = deprecatedRoles
        };
    }

    #endregion

    #region Role Permission Management

    public async Task AddPermissionToRoleAsync(RoleId roleId, PermissionId permissionId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding permission {PermissionId} to role {RoleId}", permissionId, roleId);

        var role = await _roleRepository.GetAsync(roleId, cancellationToken);
        if (role is null)
        {
            throw new Mamey.Government.Identity.Domain.Exceptions.RoleNotFoundException(roleId);
        }

        var permission = await _permissionRepository.GetAsync(permissionId, cancellationToken);
        if (permission is null)
        {
            throw new PermissionNotFoundException(permissionId);
        }

        role.AddPermission(permissionId);
        await _roleRepository.UpdateAsync(role, cancellationToken);
        await _eventProcessor.ProcessAsync(role.Events);
    }

    public async Task RemovePermissionFromRoleAsync(RoleId roleId, PermissionId permissionId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Removing permission {PermissionId} from role {RoleId}", permissionId, roleId);

        var role = await _roleRepository.GetAsync(roleId, cancellationToken);
        if (role is null)
        {
            throw new Mamey.Government.Identity.Domain.Exceptions.RoleNotFoundException(roleId);
        }

        role.RemovePermission(permissionId);
        await _roleRepository.UpdateAsync(role, cancellationToken);
        await _eventProcessor.ProcessAsync(role.Events);
    }

    #endregion

    #region Private Helper Methods

    private static RoleDto MapToRoleDto(Role role)
    {
        return new RoleDto(
            role.Id,
            role.Name,
            role.Description,
            role.Status.ToString(),
            role.Permissions.Select(p => p.Value),
            role.CreatedAt,
            role.ModifiedAt
        );
    }

    #endregion
}

