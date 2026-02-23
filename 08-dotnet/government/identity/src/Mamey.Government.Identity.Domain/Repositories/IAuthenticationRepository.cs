using System;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Repositories;

/// <summary>
/// Repository for authentication-related queries that span multiple aggregates.
/// </summary>
internal interface IAuthenticationRepository
{
    // User authentication queries
    Task<User?> AuthenticateByUsernameAsync(string username, string passwordHash, CancellationToken cancellationToken = default);
    Task<User?> AuthenticateByEmailAsync(string email, string passwordHash, CancellationToken cancellationToken = default);
    Task<User?> AuthenticateBySubjectIdAsync(SubjectId subjectId, string passwordHash, CancellationToken cancellationToken = default);
    
    // Session management queries
    Task<Session?> GetValidSessionAsync(string accessToken, CancellationToken cancellationToken = default);
    Task<Session?> GetValidRefreshSessionAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Session?>> GetActiveSessionsForUserAsync(UserId userId, CancellationToken cancellationToken = default);
    
    // Credential verification queries
    Task<Credential?> GetActiveCredentialAsync(UserId userId, CredentialType type, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Credential>> GetActiveCredentialsForUserAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<bool> HasActiveCredentialOfTypeAsync(UserId userId, CredentialType type, CancellationToken cancellationToken = default);
    
    // Security queries
    Task<bool> IsUserLockedAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<bool> CanUserAuthenticateAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<int> GetFailedLoginAttemptsAsync(UserId userId, CancellationToken cancellationToken = default);
    
    // Email confirmation queries
    Task<EmailConfirmation?> GetPendingEmailConfirmationAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<EmailConfirmation?> GetEmailConfirmationByCodeAsync(string confirmationCode, CancellationToken cancellationToken = default);
    Task<bool> HasPendingEmailConfirmationAsync(UserId userId, CancellationToken cancellationToken = default);
    
    // Two-factor authentication queries
    Task<TwoFactorAuth?> GetTwoFactorAuthAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<bool> HasTwoFactorAuthAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<bool> IsTwoFactorAuthActiveAsync(UserId userId, CancellationToken cancellationToken = default);
    
    // Multi-factor authentication queries
    Task<MultiFactorAuth?> GetMultiFactorAuthAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<bool> HasMultiFactorAuthAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<bool> IsMultiFactorAuthActiveAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MfaMethod>> GetEnabledMfaMethodsAsync(UserId userId, CancellationToken cancellationToken = default);
    
    // MFA challenge queries
    Task<MfaChallenge?> GetActiveChallengeAsync(MultiFactorAuthId multiFactorAuthId, CancellationToken cancellationToken = default);
    Task<bool> HasActiveChallengeAsync(MultiFactorAuthId multiFactorAuthId, CancellationToken cancellationToken = default);
    
    // Cleanup operations
    Task CleanupExpiredSessionsAsync(CancellationToken cancellationToken = default);
    Task CleanupExpiredCredentialsAsync(CancellationToken cancellationToken = default);
    Task CleanupExpiredEmailConfirmationsAsync(CancellationToken cancellationToken = default);
    Task CleanupExpiredMfaChallengesAsync(CancellationToken cancellationToken = default);
    Task RevokeAllSessionsForUserAsync(UserId userId, CancellationToken cancellationToken = default);
}
