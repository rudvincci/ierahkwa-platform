using System;
using System.Collections.Generic;
using System.Linq;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Types;

namespace Mamey.Government.Identity.Infrastructure.Composite;

internal class CompositeAuditRepository : IAuditRepository
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly ICredentialRepository _credentialRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;

    public CompositeAuditRepository(
        ISubjectRepository subjectRepository,
        IUserRepository userRepository,
        ISessionRepository sessionRepository,
        ICredentialRepository credentialRepository,
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository)
    {
        _subjectRepository = subjectRepository;
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _credentialRepository = credentialRepository;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<IReadOnlyList<Subject>> GetSubjectsCreatedBetweenAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var allSubjects = await _subjectRepository.BrowseAsync(cancellationToken);
        return allSubjects.Where(s => s.CreatedAt >= from && s.CreatedAt <= to).ToList();
    }

    public async Task<IReadOnlyList<Subject>> GetSubjectsModifiedBetweenAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var allSubjects = await _subjectRepository.BrowseAsync(cancellationToken);
        return allSubjects.Where(s => s.ModifiedAt.HasValue && s.ModifiedAt.Value >= from && s.ModifiedAt.Value <= to).ToList();
    }

    public async Task<IReadOnlyList<Subject>> GetSubjectsAuthenticatedBetweenAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var allSubjects = await _subjectRepository.BrowseAsync(cancellationToken);
        return allSubjects.Where(s => s.LastAuthenticatedAt.HasValue && s.LastAuthenticatedAt.Value >= from && s.LastAuthenticatedAt.Value <= to).ToList();
    }

    public async Task<IReadOnlyList<Subject>> GetSubjectsByStatusAsync(SubjectStatus status, CancellationToken cancellationToken = default)
    {
        var allSubjects = await _subjectRepository.BrowseAsync(cancellationToken);
        return allSubjects.Where(s => s.Status == status).ToList();
    }

    public async Task<IReadOnlyList<User>> GetUsersCreatedBetweenAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var allUsers = await _userRepository.BrowseAsync(cancellationToken);
        return allUsers.Where(u => u.CreatedAt >= from && u.CreatedAt <= to).ToList();
    }

    public async Task<IReadOnlyList<User>> GetUsersWithFailedLoginsAsync(int minAttempts, CancellationToken cancellationToken = default)
    {
        var allUsers = await _userRepository.BrowseAsync(cancellationToken);
        return allUsers.Where(u => u.FailedLoginAttempts >= minAttempts).ToList();
    }

    public async Task<IReadOnlyList<User>> GetLockedUsersAsync(CancellationToken cancellationToken = default)
    {
        var allUsers = await _userRepository.BrowseAsync(cancellationToken);
        return allUsers.Where(u => u.IsLocked).ToList();
    }

    public async Task<IReadOnlyList<User>> GetUsersByStatusAsync(UserStatus status, CancellationToken cancellationToken = default)
    {
        var allUsers = await _userRepository.BrowseAsync(cancellationToken);
        return allUsers.Where(u => u.Status == status).ToList();
    }

    public async Task<IReadOnlyList<Session?>> GetSessionsCreatedBetweenAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var allSessions = await _sessionRepository.BrowseAsync(cancellationToken);
        return allSessions.Where(s => s.CreatedAt >= from && s.CreatedAt <= to).ToList();
    }

    public async Task<IReadOnlyList<Session?>> GetActiveSessionsAsync(CancellationToken cancellationToken = default)
    {
        return await _sessionRepository.GetActiveSessionsAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Session?>> GetExpiredSessionsAsync(CancellationToken cancellationToken = default)
    {
        return await _sessionRepository.GetExpiredSessionsAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Session?>> GetSessionsByIpAddressAsync(string ipAddress, CancellationToken cancellationToken = default)
    {
        var allSessions = await _sessionRepository.BrowseAsync(cancellationToken);
        return allSessions.Where(s => s.IpAddress == ipAddress).ToList();
    }

    public async Task<IReadOnlyList<Session?>> GetSessionsByUserAgentAsync(string userAgent, CancellationToken cancellationToken = default)
    {
        var allSessions = await _sessionRepository.BrowseAsync(cancellationToken);
        return allSessions.Where(s => s.UserAgent == userAgent).ToList();
    }

    public async Task<IReadOnlyList<Credential>> GetCredentialsCreatedBetweenAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var allCredentials = await _credentialRepository.BrowseAsync(cancellationToken);
        return allCredentials.Where(c => c.CreatedAt >= from && c.CreatedAt <= to).ToList();
    }

    public async Task<IReadOnlyList<Credential>> GetCredentialsByTypeAsync(CredentialType type, CancellationToken cancellationToken = default)
    {
        return await _credentialRepository.GetByTypeAsync(type, cancellationToken);
    }

    public async Task<IReadOnlyList<Credential>> GetExpiredCredentialsAsync(CancellationToken cancellationToken = default)
    {
        return await _credentialRepository.GetExpiredCredentialsAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Credential>> GetCredentialsExpiringSoonAsync(DateTime expirationThreshold, CancellationToken cancellationToken = default)
    {
        return await _credentialRepository.GetCredentialsExpiringSoonAsync(expirationThreshold, cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> GetRolesCreatedBetweenAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var allRoles = await _roleRepository.BrowseAsync(cancellationToken);
        return allRoles.Where(r => r.CreatedAt >= from && r.CreatedAt <= to).ToList();
    }

    public async Task<IReadOnlyList<Permission>> GetPermissionsCreatedBetweenAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var allPermissions = await _permissionRepository.BrowseAsync(cancellationToken);
        return allPermissions.Where(p => p.CreatedAt >= from && p.CreatedAt <= to).ToList();
    }

    public async Task<IReadOnlyList<Role>> GetRolesByStatusAsync(RoleStatus status, CancellationToken cancellationToken = default)
    {
        var allRoles = await _roleRepository.BrowseAsync(cancellationToken);
        return allRoles.Where(r => r.Status == status).ToList();
    }

    public async Task<IReadOnlyList<Permission>> GetPermissionsByStatusAsync(PermissionStatus status, CancellationToken cancellationToken = default)
    {
        var allPermissions = await _permissionRepository.BrowseAsync(cancellationToken);
        return allPermissions.Where(p => p.Status == status).ToList();
    }

    public async Task<int> GetTotalSubjectsCountAsync(CancellationToken cancellationToken = default)
    {
        var allSubjects = await _subjectRepository.BrowseAsync(cancellationToken);
        return allSubjects.Count;
    }

    public async Task<int> GetActiveSubjectsCountAsync(CancellationToken cancellationToken = default)
    {
        var subjects = await GetSubjectsByStatusAsync(SubjectStatus.Active, cancellationToken);
        return subjects.Count;
    }

    public async Task<int> GetTotalUsersCountAsync(CancellationToken cancellationToken = default)
    {
        var allUsers = await _userRepository.BrowseAsync(cancellationToken);
        return allUsers.Count;
    }

    public async Task<int> GetActiveUsersCountAsync(CancellationToken cancellationToken = default)
    {
        var users = await GetUsersByStatusAsync(UserStatus.Active, cancellationToken);
        return users.Count;
    }

    public async Task<int> GetActiveSessionsCountAsync(CancellationToken cancellationToken = default)
    {
        var sessions = await GetActiveSessionsAsync(cancellationToken);
        return sessions.Count;
    }

    public async Task<int> GetTotalRolesCountAsync(CancellationToken cancellationToken = default)
    {
        var allRoles = await _roleRepository.BrowseAsync(cancellationToken);
        return allRoles.Count;
    }

    public async Task<int> GetTotalPermissionsCountAsync(CancellationToken cancellationToken = default)
    {
        var allPermissions = await _permissionRepository.BrowseAsync(cancellationToken);
        return allPermissions.Count;
    }

    public async Task CleanupExpiredDataAsync(CancellationToken cancellationToken = default)
    {
        await _sessionRepository.DeleteExpiredSessionsAsync(cancellationToken);
        await _credentialRepository.DeleteExpiredCredentialsAsync(cancellationToken);
    }

    public async Task ArchiveOldSessionsAsync(DateTime olderThan, CancellationToken cancellationToken = default)
    {
        var expiredSessions = await _sessionRepository.GetExpiredSessionsAsync(olderThan, cancellationToken);
        foreach (var session in expiredSessions)
        {
            await _sessionRepository.DeleteAsync(session.Id, cancellationToken);
        }
    }

    public async Task ArchiveOldCredentialsAsync(DateTime olderThan, CancellationToken cancellationToken = default)
    {
        var expiredCredentials = await GetExpiredCredentialsAsync(cancellationToken);
        foreach (var credential in expiredCredentials.Where(c => c.ExpiresAt.HasValue && c.ExpiresAt.Value < olderThan))
        {
            await _credentialRepository.DeleteAsync(credential.Id, cancellationToken);
        }
    }
}

