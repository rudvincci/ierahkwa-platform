using Mamey.Government.Identity.Application.Commands;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Application.Services;

internal interface IIdentityService
{
    #region User Management
    Task<UserDto?> GetUserAsync(UserId id, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserDto> CreateUserAsync(CreateUser command, CancellationToken cancellationToken = default);
    Task<UserDto> UpdateUserAsync(UpdateUser command, CancellationToken cancellationToken = default);
    Task ChangeUserPasswordAsync(ChangeUserPassword command, CancellationToken cancellationToken = default);
    Task ActivateUserAsync(ActivateUser command, CancellationToken cancellationToken = default);
    Task DeactivateUserAsync(DeactivateUser command, CancellationToken cancellationToken = default);
    Task LockUserAsync(LockUser command, CancellationToken cancellationToken = default);
    Task UnlockUserAsync(UnlockUser command, CancellationToken cancellationToken = default);
    #endregion

    #region Authentication
    Task<AuthDto?> SignInAsync(string usernameOrEmail, string password, string? ipAddress = null, string? userAgent = null, CancellationToken cancellationToken = default);
    Task<AuthDto> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task SignOutAsync(string refreshToken, CancellationToken cancellationToken = default);
    #endregion

    #region Email Confirmation
    Task<string> CreateEmailConfirmationAsync(Guid userId, string email, string? ipAddress = null, string? userAgent = null, CancellationToken cancellationToken = default);
    Task ConfirmEmailAsync(string confirmationCode, CancellationToken cancellationToken = default);
    Task ResendEmailConfirmationAsync(Guid userId, CancellationToken cancellationToken = default);
    #endregion

    #region Two-Factor Authentication
    Task<TwoFactorSetupDto> SetupTwoFactorAuthAsync(Guid userId, CancellationToken cancellationToken = default);
    Task ActivateTwoFactorAuthAsync(Guid userId, string totpCode, CancellationToken cancellationToken = default);
    Task<bool> VerifyTwoFactorAuthAsync(Guid userId, string totpCode, CancellationToken cancellationToken = default);
    Task DisableTwoFactorAuthAsync(Guid userId, CancellationToken cancellationToken = default);
    #endregion

    #region Multi-Factor Authentication
    Task<MultiFactorSetupDto> SetupMultiFactorAuthAsync(Guid userId, IEnumerable<int>? enabledMethods = null, int requiredMethods = 2, CancellationToken cancellationToken = default);
    Task EnableMfaMethodAsync(Guid userId, int method, CancellationToken cancellationToken = default);
    Task DisableMfaMethodAsync(Guid userId, int method, CancellationToken cancellationToken = default);
    Task<string> CreateMfaChallengeAsync(Guid multiFactorAuthId, int method, string challengeData, string? ipAddress = null, string? userAgent = null, CancellationToken cancellationToken = default);
    Task<bool> VerifyMfaChallengeAsync(Guid challengeId, string response, CancellationToken cancellationToken = default);
    #endregion

    #region Role Management
    Task<RoleDto> CreateRoleAsync(CreateRole command, CancellationToken cancellationToken = default);
    Task<RoleDto> UpdateRoleAsync(UpdateRole command, CancellationToken cancellationToken = default);
    Task AssignRoleToSubjectAsync(AssignRoleToSubject command, CancellationToken cancellationToken = default);
    Task RemoveRoleFromSubjectAsync(RemoveRoleFromSubject command, CancellationToken cancellationToken = default);
    #endregion

    #region Permission Management
    Task<PermissionDto> CreatePermissionAsync(CreatePermission command, CancellationToken cancellationToken = default);
    Task<PermissionDto> UpdatePermissionAsync(UpdatePermission command, CancellationToken cancellationToken = default);
    #endregion
}
