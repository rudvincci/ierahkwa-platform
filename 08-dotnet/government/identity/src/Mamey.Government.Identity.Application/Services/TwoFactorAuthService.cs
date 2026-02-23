using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Government.Identity.Application.Commands;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Events;
using Mamey.Government.Identity.Domain.Exceptions;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Time;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Application.Services;

internal sealed class TwoFactorAuthService : ITwoFactorAuthService
{
    #region Read-only Fields

    private readonly ILogger<TwoFactorAuthService> _logger;
    private readonly ITwoFactorAuthRepository _twoFactorAuthRepository;
    private readonly IUserRepository _userRepository;
    private readonly IClock _clock;
    private readonly IMessageBroker _messageBroker;
    private readonly IEventProcessor _eventProcessor;
    private readonly ICommandDispatcher _commandDispatcher;

    #endregion

    public TwoFactorAuthService(
        ITwoFactorAuthRepository twoFactorAuthRepository,
        IUserRepository userRepository,
        IClock clock,
        IMessageBroker messageBroker,
        IEventProcessor eventProcessor,
        ICommandDispatcher commandDispatcher,
        ILogger<TwoFactorAuthService> logger)
    {
        _twoFactorAuthRepository = twoFactorAuthRepository;
        _userRepository = userRepository;
        _clock = clock;
        _messageBroker = messageBroker;
        _eventProcessor = eventProcessor;
        _commandDispatcher = commandDispatcher;
        _logger = logger;
    }

    #region Two-Factor Authentication Operations

    public async Task<TwoFactorSetupDto> SetupTwoFactorAuthAsync(SetupTwoFactorAuth command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Setting up 2FA for user: {UserId}", command.UserId);

        var user = await _userRepository.GetAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundException(command.UserId);
        }

        // Check if user already has 2FA setup
        var existing2FA = await _twoFactorAuthRepository.GetByUserIdAsync(command.UserId, cancellationToken);
        if (existing2FA is not null)
        {
            throw new TwoFactorAlreadyEnabledException();
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);

        return new TwoFactorSetupDto
        {
            SecretKey = command.SecretKey,
            QrCodeUrl = command.QrCodeUrl
        };
    }

    public async Task ActivateTwoFactorAuthAsync(ActivateTwoFactorAuth command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Activating 2FA for user: {UserId}", command.UserId);

        var twoFactorAuth = await _twoFactorAuthRepository.GetByUserIdAsync(command.UserId, cancellationToken);
        if (twoFactorAuth is null)
        {
            throw new TwoFactorAuthNotFoundException(command.UserId);
        }

        if (twoFactorAuth.Status == TwoFactorAuthStatus.Active)
        {
            throw new TwoFactorAlreadyActiveException(command.UserId);
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);

        var user = await _userRepository.GetAsync(twoFactorAuth.UserId, cancellationToken);
        await _messageBroker.PublishAsync(new Mamey.Government.Identity.Application.Events.UserTwoFactorEnabled(user, twoFactorAuth));
    }

    public async Task<bool> VerifyTwoFactorAuthAsync(VerifyTwoFactorAuth command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Verifying 2FA for user: {UserId}", command.UserId);

        var twoFactorAuth = await _twoFactorAuthRepository.GetByUserIdAsync(command.UserId, cancellationToken);
        if (twoFactorAuth is null)
        {
            throw new TwoFactorAuthNotFoundException(command.UserId);
        }

        if (twoFactorAuth.Status != TwoFactorAuthStatus.Active)
        {
            throw new TwoFactorNotActiveException();
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);

        return true; // If no exception is thrown, verification succeeded
    }

    public async Task DisableTwoFactorAuthAsync(DisableTwoFactorAuth command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Disabling 2FA for user: {UserId}", command.UserId);

        var twoFactorAuth = await _twoFactorAuthRepository.GetByUserIdAsync(command.UserId, cancellationToken);
        if (twoFactorAuth is null)
        {
            throw new TwoFactorAuthNotFoundException(command.UserId);
        }

        if (twoFactorAuth.Status == TwoFactorAuthStatus.Disabled)
        {
            throw new TwoFactorAlreadyDisabledException(command.UserId);
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);

        await _messageBroker.PublishAsync(new Mamey.Government.Identity.Application.Events.UserTwoFactorDisabled(command.UserId, _clock.CurrentDate()));
    }

    #endregion

    #region Two-Factor Authentication Queries

    public async Task<TwoFactorAuthDto?> GetTwoFactorAuthAsync(TwoFactorAuthId id, CancellationToken cancellationToken = default)
    {
        var twoFactorAuth = await _twoFactorAuthRepository.GetAsync(id, cancellationToken);
        return twoFactorAuth is null ? null : MapToTwoFactorAuthDto(twoFactorAuth);
    }

    public async Task<TwoFactorAuthDto?> GetTwoFactorAuthByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var twoFactorAuth = await _twoFactorAuthRepository.GetByUserIdAsync(userId, cancellationToken);
        return twoFactorAuth is null ? null : MapToTwoFactorAuthDto(twoFactorAuth);
    }

    public async Task<bool> HasTwoFactorAuthAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var twoFactorAuth = await _twoFactorAuthRepository.GetByUserIdAsync(userId, cancellationToken);
        return twoFactorAuth is not null && twoFactorAuth.Status == TwoFactorAuthStatus.Active;
    }

    public async Task<bool> IsTwoFactorAuthActiveAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var twoFactorAuth = await _twoFactorAuthRepository.GetByUserIdAsync(userId, cancellationToken);
        return twoFactorAuth is not null && twoFactorAuth.Status == TwoFactorAuthStatus.Active;
    }

    #endregion

    #region Two-Factor Authentication Management

    public async Task GenerateBackupCodesAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating backup codes for user: {UserId}", userId);

        var twoFactorAuth = await _twoFactorAuthRepository.GetByUserIdAsync(userId, cancellationToken);
        if (twoFactorAuth is null)
        {
            throw new TwoFactorAuthNotFoundException(userId);
        }

        twoFactorAuth.GenerateBackupCodes();
        await _twoFactorAuthRepository.UpdateAsync(twoFactorAuth, cancellationToken);
        await _eventProcessor.ProcessAsync(twoFactorAuth.Events);
    }

    public async Task<bool> VerifyBackupCodeAsync(UserId userId, string backupCode, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Verifying backup code for user: {UserId}", userId);

        var twoFactorAuth = await _twoFactorAuthRepository.GetByUserIdAsync(userId, cancellationToken);
        if (twoFactorAuth is null)
        {
            throw new TwoFactorAuthNotFoundException(userId);
        }

        var isValid = twoFactorAuth.VerifyBackupCode(backupCode);
        if (isValid)
        {
            await _twoFactorAuthRepository.UpdateAsync(twoFactorAuth, cancellationToken);
            await _eventProcessor.ProcessAsync(twoFactorAuth.Events);
        }

        return isValid;
    }

    public async Task RegenerateSecretKeyAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Regenerating secret key for user: {UserId}", userId);

        var twoFactorAuth = await _twoFactorAuthRepository.GetByUserIdAsync(userId, cancellationToken);
        if (twoFactorAuth is null)
        {
            throw new TwoFactorAuthNotFoundException(userId);
        }

        twoFactorAuth.RegenerateSecretKey();
        var newQrCodeUrl = GenerateQrCodeUrl(userId, twoFactorAuth.SecretKey);

        await _twoFactorAuthRepository.UpdateAsync(twoFactorAuth, cancellationToken);
        await _eventProcessor.ProcessAsync(twoFactorAuth.Events);
    }

    #endregion

    #region Two-Factor Authentication Statistics

    public async Task<TwoFactorAuthStatisticsDto> GetTwoFactorAuthStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var total2FA = await _twoFactorAuthRepository.CountAsync(cancellationToken);
        var active2FA = await _twoFactorAuthRepository.CountByStatusAsync(TwoFactorAuthStatus.Active, cancellationToken);
        var pending2FA = await _twoFactorAuthRepository.CountByStatusAsync(TwoFactorAuthStatus.Pending, cancellationToken);
        var disabled2FA = await _twoFactorAuthRepository.CountByStatusAsync(TwoFactorAuthStatus.Disabled, cancellationToken);

        return new TwoFactorAuthStatisticsDto
        {
            TotalTwoFactorAuth = total2FA,
            ActiveTwoFactorAuth = active2FA,
            PendingTwoFactorAuth = pending2FA,
            DisabledTwoFactorAuth = disabled2FA
        };
    }

    #endregion

    #region Private Helper Methods

    private static TwoFactorAuthDto MapToTwoFactorAuthDto(TwoFactorAuth twoFactorAuth)
    {
        return new TwoFactorAuthDto(
            twoFactorAuth.Id,
            twoFactorAuth.UserId,
            twoFactorAuth.SecretKey,
            twoFactorAuth.QrCodeUrl,
            twoFactorAuth.BackupCodes,
            twoFactorAuth.Status.ToString(),
            twoFactorAuth.CreatedAt,
            twoFactorAuth.ActivatedAt
        );
    }

    private static string GenerateSecretKey()
    {
        return Guid.NewGuid().ToString("N")[..16].ToUpperInvariant();
    }

    private static string GenerateQrCodeUrl(UserId userId, string secretKey)
    {
        return $"otpauth://totp/GovernmentIdentity:{userId}?secret={secretKey}&issuer=GovernmentIdentity";
    }

    #endregion
}

