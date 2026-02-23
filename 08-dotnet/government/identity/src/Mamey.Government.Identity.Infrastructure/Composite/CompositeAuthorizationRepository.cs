using System;
using System.Collections.Generic;
using System.Linq;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Types;

namespace Mamey.Government.Identity.Infrastructure.Composite;

internal class CompositeAuthorizationRepository : IAuthorizationRepository
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;

    public CompositeAuthorizationRepository(
        ISubjectRepository subjectRepository,
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository)
    {
        _subjectRepository = subjectRepository;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<bool> HasPermissionAsync(SubjectId subjectId, string resource, string action, CancellationToken cancellationToken = default)
    {
        var subject = await _subjectRepository.GetAsync(subjectId, cancellationToken);
        if (subject == null) return false;

        var permissions = await _permissionRepository.GetByResourceAndActionAsync(resource, action, cancellationToken);
        if (permissions == null || !permissions.Any()) return false;
        
        // Check if any of the subject's roles have any of these permissions
        var roles = await GetRolesForSubjectAsync(subjectId, cancellationToken);
        var permissionIds = permissions.Select(p => p.Id).ToHashSet();
        return roles.Any(r => r.Permissions.Any(p => permissionIds.Contains(p)));
    }

    public async Task<bool> HasRoleAsync(SubjectId subjectId, RoleId roleId, CancellationToken cancellationToken = default)
    {
        var subject = await _subjectRepository.GetAsync(subjectId, cancellationToken);
        return subject?.HasRole(roleId) ?? false;
    }

    public async Task<bool> HasAnyRoleAsync(SubjectId subjectId, IEnumerable<RoleId> roleIds, CancellationToken cancellationToken = default)
    {
        var subject = await _subjectRepository.GetAsync(subjectId, cancellationToken);
        if (subject == null) return false;
        return roleIds.Any(roleId => subject.HasRole(roleId));
    }

    public async Task<bool> HasAllRolesAsync(SubjectId subjectId, IEnumerable<RoleId> roleIds, CancellationToken cancellationToken = default)
    {
        var subject = await _subjectRepository.GetAsync(subjectId, cancellationToken);
        if (subject == null) return false;
        return roleIds.All(roleId => subject.HasRole(roleId));
    }

    public async Task<IReadOnlyList<Role>> GetRolesForSubjectAsync(SubjectId subjectId, CancellationToken cancellationToken = default)
    {
        var subject = await _subjectRepository.GetAsync(subjectId, cancellationToken);
        if (subject == null) return new List<Role>();

        var roles = new List<Role>();
        foreach (var roleId in subject.Roles)
        {
            var role = await _roleRepository.GetAsync(roleId, cancellationToken);
            if (role != null)
            {
                roles.Add(role);
            }
        }
        return roles;
    }

    public async Task<IReadOnlyList<Permission>> GetPermissionsForSubjectAsync(SubjectId subjectId, CancellationToken cancellationToken = default)
    {
        var roles = await GetRolesForSubjectAsync(subjectId, cancellationToken);
        var permissionIds = roles.SelectMany(r => r.Permissions).Distinct();
        var permissions = new List<Permission>();

        foreach (var permissionId in permissionIds)
        {
            var permission = await _permissionRepository.GetAsync(permissionId, cancellationToken);
            if (permission != null)
            {
                permissions.Add(permission);
            }
        }
        return permissions;
    }

    public async Task<IReadOnlyList<Permission>> GetPermissionsForRoleAsync(RoleId roleId, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetAsync(roleId, cancellationToken);
        if (role == null) return new List<Permission>();

        var permissions = new List<Permission>();
        foreach (var permissionId in role.Permissions)
        {
            var permission = await _permissionRepository.GetAsync(permissionId, cancellationToken);
            if (permission != null)
            {
                permissions.Add(permission);
            }
        }
        return permissions;
    }

    public async Task<IReadOnlyList<Role>> GetRolesWithPermissionAsync(PermissionId permissionId, CancellationToken cancellationToken = default)
    {
        var allRoles = await _roleRepository.BrowseAsync(cancellationToken);
        return allRoles.Where(r => r.HasPermission(permissionId)).ToList();
    }

    public async Task<bool> PermissionExistsAsync(string resource, string action, CancellationToken cancellationToken = default)
    {
        var permissions = await _permissionRepository.GetByResourceAndActionAsync(resource, action, cancellationToken);
        return permissions != null && permissions.Any();
    }

    public async Task<Permission?> GetPermissionByResourceAndActionAsync(string resource, string action, CancellationToken cancellationToken = default)
    {
        var permissions = await _permissionRepository.GetByResourceAndActionAsync(resource, action, cancellationToken);
        return permissions?.FirstOrDefault();
    }

    public async Task<IReadOnlyList<Permission>> GetPermissionsByResourceAsync(string resource, CancellationToken cancellationToken = default)
    {
        return await _permissionRepository.GetByResourceAsync(resource, cancellationToken);
    }

    public async Task<IReadOnlyList<Permission>> GetPermissionsByActionAsync(string action, CancellationToken cancellationToken = default)
    {
        return await _permissionRepository.GetByActionAsync(action, cancellationToken);
    }

    public async Task<bool> RoleExistsAsync(string roleName, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByNameAsync(roleName, cancellationToken);
        return role != null;
    }

    public async Task<Role?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        return await _roleRepository.GetByNameAsync(roleName, cancellationToken);
    }

    public async Task<IReadOnlyList<Subject>> GetSubjectsWithRoleAsync(RoleId roleId, CancellationToken cancellationToken = default)
    {
        var allSubjects = await _subjectRepository.BrowseAsync(cancellationToken);
        return allSubjects.Where(s => s.HasRole(roleId)).ToList();
    }

    public async Task<IReadOnlyList<Subject>> GetSubjectsWithAnyRoleAsync(IEnumerable<RoleId> roleIds, CancellationToken cancellationToken = default)
    {
        var allSubjects = await _subjectRepository.BrowseAsync(cancellationToken);
        var roleIdSet = roleIds.ToHashSet();
        return allSubjects.Where(s => s.Roles.Any(r => roleIdSet.Contains(r))).ToList();
    }

    public async Task AssignRoleToSubjectAsync(SubjectId subjectId, RoleId roleId, CancellationToken cancellationToken = default)
    {
        var subject = await _subjectRepository.GetAsync(subjectId, cancellationToken);
        if (subject == null) throw new ArgumentException($"Subject {subjectId} not found");

        subject.AddRole(roleId);
        await _subjectRepository.UpdateAsync(subject, cancellationToken);
    }

    public async Task RemoveRoleFromSubjectAsync(SubjectId subjectId, RoleId roleId, CancellationToken cancellationToken = default)
    {
        var subject = await _subjectRepository.GetAsync(subjectId, cancellationToken);
        if (subject == null) throw new ArgumentException($"Subject {subjectId} not found");

        subject.RemoveRole(roleId);
        await _subjectRepository.UpdateAsync(subject, cancellationToken);
    }

    public async Task AssignPermissionToRoleAsync(RoleId roleId, PermissionId permissionId, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetAsync(roleId, cancellationToken);
        if (role == null) throw new ArgumentException($"Role {roleId} not found");

        role.AddPermission(permissionId);
        await _roleRepository.UpdateAsync(role, cancellationToken);
    }

    public async Task RemovePermissionFromRoleAsync(RoleId roleId, PermissionId permissionId, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetAsync(roleId, cancellationToken);
        if (role == null) throw new ArgumentException($"Role {roleId} not found");

        role.RemovePermission(permissionId);
        await _roleRepository.UpdateAsync(role, cancellationToken);
    }
}

