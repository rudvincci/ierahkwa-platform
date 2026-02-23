using System;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Repositories;

/// <summary>
/// Repository for audit and reporting queries across all aggregates.
/// </summary>
internal interface IAuditRepository
{
    // Subject audit queries
    Task<IReadOnlyList<Subject>> GetSubjectsCreatedBetweenAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Subject>> GetSubjectsModifiedBetweenAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Subject>> GetSubjectsAuthenticatedBetweenAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Subject>> GetSubjectsByStatusAsync(SubjectStatus status, CancellationToken cancellationToken = default);
    
    // User audit queries
    Task<IReadOnlyList<User>> GetUsersCreatedBetweenAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetUsersWithFailedLoginsAsync(int minAttempts, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetLockedUsersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetUsersByStatusAsync(UserStatus status, CancellationToken cancellationToken = default);
    
    // Session audit queries
    Task<IReadOnlyList<Session?>> GetSessionsCreatedBetweenAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Session?>> GetActiveSessionsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Session?>> GetExpiredSessionsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Session?>> GetSessionsByIpAddressAsync(string ipAddress, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Session?>> GetSessionsByUserAgentAsync(string userAgent, CancellationToken cancellationToken = default);
    
    // Credential audit queries
    Task<IReadOnlyList<Credential>> GetCredentialsCreatedBetweenAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Credential>> GetCredentialsByTypeAsync(CredentialType type, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Credential>> GetExpiredCredentialsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Credential>> GetCredentialsExpiringSoonAsync(DateTime expirationThreshold, CancellationToken cancellationToken = default);
    
    // Role and permission audit queries
    Task<IReadOnlyList<Role>> GetRolesCreatedBetweenAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetPermissionsCreatedBetweenAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetRolesByStatusAsync(RoleStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetPermissionsByStatusAsync(PermissionStatus status, CancellationToken cancellationToken = default);
    
    // Statistics queries
    Task<int> GetTotalSubjectsCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetActiveSubjectsCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetTotalUsersCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetActiveUsersCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetActiveSessionsCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetTotalRolesCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetTotalPermissionsCountAsync(CancellationToken cancellationToken = default);
    
    // Cleanup operations
    Task CleanupExpiredDataAsync(CancellationToken cancellationToken = default);
    Task ArchiveOldSessionsAsync(DateTime olderThan, CancellationToken cancellationToken = default);
    Task ArchiveOldCredentialsAsync(DateTime olderThan, CancellationToken cancellationToken = default);
}
