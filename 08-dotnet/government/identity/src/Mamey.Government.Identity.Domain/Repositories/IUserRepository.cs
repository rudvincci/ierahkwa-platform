using System;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Repositories;

internal interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteAsync(UserId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<User> GetAsync(UserId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(UserId id, CancellationToken cancellationToken = default);
    
    // Authentication-specific queries
    Task<User> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User> GetBySubjectIdAsync(SubjectId subjectId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetLockedUsersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetUsersWithFailedAttemptsAsync(int minAttempts, CancellationToken cancellationToken = default);
    Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> SubjectIdExistsAsync(SubjectId subjectId, CancellationToken cancellationToken = default);
    
    // Statistics and counting methods
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountByStatusAsync(UserStatus status, CancellationToken cancellationToken = default);
    Task<int> CountLockedAsync(CancellationToken cancellationToken = default);
    Task<int> CountWithTwoFactorAsync(CancellationToken cancellationToken = default);
    Task<int> CountWithMultiFactorAsync(CancellationToken cancellationToken = default);
    
    // Additional query methods
    Task<IReadOnlyList<User>> GetByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetRecentlyAuthenticatedAsync(DateTime since, CancellationToken cancellationToken = default);
}
