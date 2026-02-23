using System;
using System.Collections.Generic;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Types;

namespace Mamey.Government.Identity.Infrastructure.Composite;

internal class CompositeAuthenticationRepository : IAuthenticationRepository
{
    private readonly IUserRepository _userRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly ICredentialRepository _credentialRepository;
    private readonly IEmailConfirmationRepository _emailConfirmationRepository;
    private readonly ITwoFactorAuthRepository _twoFactorAuthRepository;
    private readonly IMultiFactorAuthRepository _multiFactorAuthRepository;
    private readonly IMfaChallengeRepository _mfaChallengeRepository;

    public CompositeAuthenticationRepository(
        IUserRepository userRepository,
        ISessionRepository sessionRepository,
        ICredentialRepository credentialRepository,
        IEmailConfirmationRepository emailConfirmationRepository,
        ITwoFactorAuthRepository twoFactorAuthRepository,
        IMultiFactorAuthRepository multiFactorAuthRepository,
        IMfaChallengeRepository mfaChallengeRepository)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _credentialRepository = credentialRepository;
        _emailConfirmationRepository = emailConfirmationRepository;
        _twoFactorAuthRepository = twoFactorAuthRepository;
        _multiFactorAuthRepository = multiFactorAuthRepository;
        _mfaChallengeRepository = mfaChallengeRepository;
    }

    public async Task<User?> AuthenticateByUsernameAsync(string username, string passwordHash, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByUsernameAsync(username, cancellationToken);
        if (user == null || user.PasswordHash != passwordHash) return null;
        return user.CanAuthenticate() ? user : null;
    }

    public async Task<User?> AuthenticateByEmailAsync(string email, string passwordHash, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user == null || user.PasswordHash != passwordHash) return null;
        return user.CanAuthenticate() ? user : null;
    }

    public async Task<User?> AuthenticateBySubjectIdAsync(SubjectId subjectId, string passwordHash, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetBySubjectIdAsync(subjectId, cancellationToken);
        if (user == null || user.PasswordHash != passwordHash) return null;
        return user.CanAuthenticate() ? user : null;
    }

    public async Task<Session?> GetValidSessionAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByAccessTokenAsync(accessToken, cancellationToken);
        return session != null && session.IsActive() ? session : null;
    }

    public async Task<Session?> GetValidRefreshSessionAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByRefreshTokenAsync(refreshToken, cancellationToken);
        return session != null && session.IsActive() ? session : null;
    }

    public async Task<IReadOnlyList<Session?>> GetActiveSessionsForUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var allSessions = await _sessionRepository.GetByUserIdAsync(userId, cancellationToken);
        return allSessions.Where(s => s.IsActive()).ToList();
    }

    public async Task<Credential?> GetActiveCredentialAsync(UserId userId, CredentialType type, CancellationToken cancellationToken = default)
    {
        var credentials = await _credentialRepository.GetByUserIdAndTypeAsync(userId, type, cancellationToken);
        var now = DateTime.UtcNow;
        return credentials.FirstOrDefault(c => c.Status == CredentialStatus.Active && 
                                              (!c.ExpiresAt.HasValue || c.ExpiresAt.Value > now));
    }

    public async Task<IReadOnlyList<Credential>> GetActiveCredentialsForUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _credentialRepository.GetActiveCredentialsAsync(userId, cancellationToken);
    }

    public async Task<bool> HasActiveCredentialOfTypeAsync(UserId userId, CredentialType type, CancellationToken cancellationToken = default)
    {
        return await _credentialRepository.HasActiveCredentialOfTypeAsync(userId, type, cancellationToken);
    }

    public async Task<bool> IsUserLockedAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetAsync(userId, cancellationToken);
        return user?.IsLocked ?? false;
    }

    public async Task<bool> CanUserAuthenticateAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetAsync(userId, cancellationToken);
        return user?.CanAuthenticate() ?? false;
    }

    public async Task<int> GetFailedLoginAttemptsAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetAsync(userId, cancellationToken);
        return user?.FailedLoginAttempts ?? 0;
    }

    public async Task<EmailConfirmation?> GetPendingEmailConfirmationAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _emailConfirmationRepository.GetByUserIdAsync(userId, cancellationToken);
    }

    public async Task<EmailConfirmation?> GetEmailConfirmationByCodeAsync(string confirmationCode, CancellationToken cancellationToken = default)
    {
        return await _emailConfirmationRepository.GetByConfirmationCodeAsync(confirmationCode, cancellationToken);
    }

    public async Task<bool> HasPendingEmailConfirmationAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _emailConfirmationRepository.HasPendingConfirmationAsync(userId, cancellationToken);
    }

    public async Task<TwoFactorAuth?> GetTwoFactorAuthAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _twoFactorAuthRepository.GetByUserIdAsync(userId, cancellationToken);
    }

    public async Task<bool> HasTwoFactorAuthAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var twoFactor = await GetTwoFactorAuthAsync(userId, cancellationToken);
        return twoFactor != null;
    }

    public async Task<bool> IsTwoFactorAuthActiveAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _twoFactorAuthRepository.HasActiveTwoFactorAuthAsync(userId, cancellationToken);
    }

    public async Task<MultiFactorAuth?> GetMultiFactorAuthAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _multiFactorAuthRepository.GetByUserIdAsync(userId, cancellationToken);
    }

    public async Task<bool> HasMultiFactorAuthAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var multiFactor = await GetMultiFactorAuthAsync(userId, cancellationToken);
        return multiFactor != null;
    }

    public async Task<bool> IsMultiFactorAuthActiveAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _multiFactorAuthRepository.HasActiveMultiFactorAuthAsync(userId, cancellationToken);
    }

    public async Task<IReadOnlyList<MfaMethod>> GetEnabledMfaMethodsAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var multiFactor = await GetMultiFactorAuthAsync(userId, cancellationToken);
        return multiFactor?.EnabledMethods.ToList() ?? new List<MfaMethod>();
    }

    public async Task<MfaChallenge?> GetActiveChallengeAsync(MultiFactorAuthId multiFactorAuthId, CancellationToken cancellationToken = default)
    {
        var challenge = await _mfaChallengeRepository.GetByMultiFactorAuthIdAsync(multiFactorAuthId, cancellationToken);
        if (challenge != null && challenge.Status == MfaChallengeStatus.Pending && challenge.ExpiresAt > DateTime.UtcNow)
        {
            return challenge;
        }
        return null;
    }

    public async Task<bool> HasActiveChallengeAsync(MultiFactorAuthId multiFactorAuthId, CancellationToken cancellationToken = default)
    {
        return await _mfaChallengeRepository.HasActiveChallengeAsync(multiFactorAuthId, cancellationToken);
    }

    public async Task CleanupExpiredSessionsAsync(CancellationToken cancellationToken = default)
    {
        await _sessionRepository.DeleteExpiredSessionsAsync(cancellationToken);
    }

    public async Task CleanupExpiredCredentialsAsync(CancellationToken cancellationToken = default)
    {
        await _credentialRepository.DeleteExpiredCredentialsAsync(cancellationToken);
    }

    public async Task CleanupExpiredEmailConfirmationsAsync(CancellationToken cancellationToken = default)
    {
        await _emailConfirmationRepository.DeleteExpiredConfirmationsAsync(cancellationToken);
    }

    public async Task CleanupExpiredMfaChallengesAsync(CancellationToken cancellationToken = default)
    {
        await _mfaChallengeRepository.DeleteExpiredChallengesAsync(cancellationToken);
    }

    public async Task RevokeAllSessionsForUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var sessions = await GetActiveSessionsForUserAsync(userId, cancellationToken);
        foreach (var session in sessions)
        {
            await _sessionRepository.DeleteAsync(session.Id, cancellationToken);
        }
    }
}

