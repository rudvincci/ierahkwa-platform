using Mamey.Government.Identity.Application.Commands;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Application.Services;

internal interface ITwoFactorAuthService
{
    #region Two-Factor Authentication Operations
    Task<TwoFactorSetupDto> SetupTwoFactorAuthAsync(SetupTwoFactorAuth command, CancellationToken cancellationToken = default);
    Task ActivateTwoFactorAuthAsync(ActivateTwoFactorAuth command, CancellationToken cancellationToken = default);
    Task<bool> VerifyTwoFactorAuthAsync(VerifyTwoFactorAuth command, CancellationToken cancellationToken = default);
    Task DisableTwoFactorAuthAsync(DisableTwoFactorAuth command, CancellationToken cancellationToken = default);
    #endregion

    #region Two-Factor Authentication Queries
    Task<TwoFactorAuthDto?> GetTwoFactorAuthAsync(TwoFactorAuthId id, CancellationToken cancellationToken = default);
    Task<TwoFactorAuthDto?> GetTwoFactorAuthByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<bool> HasTwoFactorAuthAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<bool> IsTwoFactorAuthActiveAsync(UserId userId, CancellationToken cancellationToken = default);
    #endregion

    #region Two-Factor Authentication Management
    Task GenerateBackupCodesAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<bool> VerifyBackupCodeAsync(UserId userId, string backupCode, CancellationToken cancellationToken = default);
    Task RegenerateSecretKeyAsync(UserId userId, CancellationToken cancellationToken = default);
    #endregion

    #region Two-Factor Authentication Statistics
    Task<TwoFactorAuthStatisticsDto> GetTwoFactorAuthStatisticsAsync(CancellationToken cancellationToken = default);
    #endregion
}
