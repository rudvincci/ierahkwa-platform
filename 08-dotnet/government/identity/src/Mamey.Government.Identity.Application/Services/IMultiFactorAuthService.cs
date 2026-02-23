using Mamey.Government.Identity.Application.Commands;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Application.Services;

internal interface IMultiFactorAuthService
{
    #region Multi-Factor Authentication Operations
    Task<MultiFactorSetupDto> SetupMultiFactorAuthAsync(SetupMultiFactorAuth command, CancellationToken cancellationToken = default);
    Task EnableMfaMethodAsync(EnableMfaMethod command, CancellationToken cancellationToken = default);
    Task DisableMfaMethodAsync(DisableMfaMethod command, CancellationToken cancellationToken = default);
    Task<string> CreateMfaChallengeAsync(CreateMfaChallenge command, CancellationToken cancellationToken = default);
    Task<bool> VerifyMfaChallengeAsync(VerifyMfaChallenge command, CancellationToken cancellationToken = default);
    #endregion

    #region Multi-Factor Authentication Queries
    Task<MultiFactorAuthDto?> GetMultiFactorAuthAsync(MultiFactorAuthId id, CancellationToken cancellationToken = default);
    Task<MultiFactorAuthDto?> GetMultiFactorAuthByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<bool> HasMultiFactorAuthAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<bool> IsMultiFactorAuthActiveAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<int>> GetEnabledMfaMethodsAsync(UserId userId, CancellationToken cancellationToken = default);
    #endregion

    #region MFA Challenge Management
    Task<MfaChallengeDto?> GetMfaChallengeAsync(MfaChallengeId id, CancellationToken cancellationToken = default);
    Task<MfaChallengeDto?> GetActiveChallengeAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<bool> HasActiveChallengeAsync(UserId userId, CancellationToken cancellationToken = default);
    Task CleanupExpiredMfaChallengesAsync(CancellationToken cancellationToken = default);
    #endregion

    #region Multi-Factor Authentication Management
    Task UpdateRequiredMethodsAsync(UserId userId, int requiredMethods, CancellationToken cancellationToken = default);
    Task ActivateMultiFactorAuthAsync(UserId userId, CancellationToken cancellationToken = default);
    Task DeactivateMultiFactorAuthAsync(UserId userId, CancellationToken cancellationToken = default);
    #endregion

    #region Multi-Factor Authentication Statistics
    Task<MultiFactorAuthStatisticsDto> GetMultiFactorAuthStatisticsAsync(CancellationToken cancellationToken = default);
    #endregion
}
