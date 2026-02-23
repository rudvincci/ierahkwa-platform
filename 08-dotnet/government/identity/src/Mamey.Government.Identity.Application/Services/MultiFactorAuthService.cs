using System.Linq;
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
using MfaChallengeNotFoundException = Mamey.Government.Identity.Application.Exceptions.MfaChallengeNotFoundException;

namespace Mamey.Government.Identity.Application.Services;

internal sealed class MultiFactorAuthService : IMultiFactorAuthService
{
    #region Read-only Fields

    private readonly ILogger<MultiFactorAuthService> _logger;
    private readonly IMultiFactorAuthRepository _multiFactorAuthRepository;
    private readonly IMfaChallengeRepository _mfaChallengeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IClock _clock;
    private readonly IMessageBroker _messageBroker;
    private readonly IEventProcessor _eventProcessor;
    private readonly ICommandDispatcher _commandDispatcher;

    #endregion

    public MultiFactorAuthService(
        IMultiFactorAuthRepository multiFactorAuthRepository,
        IMfaChallengeRepository mfaChallengeRepository,
        IUserRepository userRepository,
        IClock clock,
        IMessageBroker messageBroker,
        IEventProcessor eventProcessor,
        ICommandDispatcher commandDispatcher,
        ILogger<MultiFactorAuthService> logger)
    {
        _multiFactorAuthRepository = multiFactorAuthRepository;
        _mfaChallengeRepository = mfaChallengeRepository;
        _userRepository = userRepository;
        _clock = clock;
        _messageBroker = messageBroker;
        _eventProcessor = eventProcessor;
        _commandDispatcher = commandDispatcher;
        _logger = logger;
    }

    #region Multi-Factor Authentication Operations

    public async Task<MultiFactorSetupDto> SetupMultiFactorAuthAsync(SetupMultiFactorAuth command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Setting up MFA for user: {UserId}", command.UserId);

        var user = await _userRepository.GetAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundException(command.UserId);
        }

        // Check if user already has MFA setup
        var existingMFA = await _multiFactorAuthRepository.GetByUserIdAsync(command.UserId, cancellationToken);
        if (existingMFA is not null)
        {
            throw new MultiFactorAlreadyEnabledException(command.UserId);
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);

        return new MultiFactorSetupDto
        {
            EnabledMethods = command.EnabledMethods,
            RequiredMethods = command.RequiredMethods
        };
    }

    public async Task EnableMfaMethodAsync(EnableMfaMethod command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Enabling MFA method {Method} for user: {UserId}", command.Method, command.UserId);

        var multiFactorAuth = await _multiFactorAuthRepository.GetByUserIdAsync(command.UserId, cancellationToken);
        if (multiFactorAuth is null)
        {
            throw new MultiFactorAuthNotFoundException(command.UserId);
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);
    }

    public async Task DisableMfaMethodAsync(DisableMfaMethod command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Disabling MFA method {Method} for user: {UserId}", command.Method, command.UserId);

        var multiFactorAuth = await _multiFactorAuthRepository.GetByUserIdAsync(command.UserId, cancellationToken);
        if (multiFactorAuth is null)
        {
            throw new MultiFactorAuthNotFoundException(command.UserId);
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);
    }

    public async Task<string> CreateMfaChallengeAsync(CreateMfaChallenge command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating MFA challenge for multiFactorAuthId: {MultiFactorAuthId}", command.MultiFactorAuthId);

        var multiFactorAuth = await _multiFactorAuthRepository.GetAsync(command.MultiFactorAuthId, cancellationToken);
        if (multiFactorAuth is null)
        {
            throw new MultiFactorAuthNotFoundException(command.MultiFactorAuthId);
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);

        return command.Id.ToString();
    }

    public async Task<bool> VerifyMfaChallengeAsync(VerifyMfaChallenge command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Verifying MFA challenge: {ChallengeId}", command.ChallengeId);

        var challenge = await _mfaChallengeRepository.GetAsync(command.ChallengeId, cancellationToken);
        if (challenge is null)
        {
            throw new MfaChallengeNotFoundException(command.ChallengeId);
        }

        if (challenge.Status != MfaChallengeStatus.Pending)
        {
            throw new MfaChallengeAlreadyVerifiedException(command.ChallengeId);
        }

        if (challenge.ExpiresAt < _clock.CurrentDate())
        {
            throw new MfaChallengeExpiredException(command.ChallengeId);
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);

        return true; // If no exception is thrown, verification succeeded
    }

    #endregion

    #region Multi-Factor Authentication Queries

    public async Task<MultiFactorAuthDto?> GetMultiFactorAuthAsync(MultiFactorAuthId id, CancellationToken cancellationToken = default)
    {
        var multiFactorAuth = await _multiFactorAuthRepository.GetAsync(id, cancellationToken);
        return multiFactorAuth is null ? null : MapToMultiFactorAuthDto(multiFactorAuth);
    }

    public async Task<MultiFactorAuthDto?> GetMultiFactorAuthByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var multiFactorAuth = await _multiFactorAuthRepository.GetByUserIdAsync(userId, cancellationToken);
        return multiFactorAuth is null ? null : MapToMultiFactorAuthDto(multiFactorAuth);
    }

    public async Task<bool> HasMultiFactorAuthAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var multiFactorAuth = await _multiFactorAuthRepository.GetByUserIdAsync(userId, cancellationToken);
        return multiFactorAuth is not null && multiFactorAuth.Status == MultiFactorAuthStatus.Active;
    }

    public async Task<bool> IsMultiFactorAuthActiveAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var multiFactorAuth = await _multiFactorAuthRepository.GetByUserIdAsync(userId, cancellationToken);
        return multiFactorAuth is not null && multiFactorAuth.Status == MultiFactorAuthStatus.Active;
    }

    public async Task<IEnumerable<int>> GetEnabledMfaMethodsAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var multiFactorAuth = await _multiFactorAuthRepository.GetByUserIdAsync(userId, cancellationToken);
        return multiFactorAuth?.EnabledMethods?.Cast<int>() ?? Enumerable.Empty<int>();
    }

    #endregion

    #region MFA Challenge Management

    public async Task<MfaChallengeDto?> GetMfaChallengeAsync(MfaChallengeId id, CancellationToken cancellationToken = default)
    {
        var challenge = await _mfaChallengeRepository.GetAsync(id, cancellationToken);
        return challenge is null ? null : MapToMfaChallengeDto(challenge);
    }

    public async Task<MfaChallengeDto?> GetActiveChallengeAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var challenge = await _mfaChallengeRepository.GetActiveByUserIdAsync(userId, cancellationToken);
        return challenge is null ? null : MapToMfaChallengeDto(challenge);
    }

    public async Task<bool> HasActiveChallengeAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var challenge = await _mfaChallengeRepository.GetActiveByUserIdAsync(userId, cancellationToken);
        return challenge is not null && challenge.Status == MfaChallengeStatus.Pending && challenge.ExpiresAt > _clock.CurrentDate();
    }

    public async Task CleanupExpiredMfaChallengesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Cleaning up expired MFA challenges");

        var expiredChallenges = await _mfaChallengeRepository.GetExpiredAsync(cancellationToken);
        
        foreach (var challenge in expiredChallenges)
        {
            challenge.Expire();
            await _mfaChallengeRepository.UpdateAsync(challenge, cancellationToken);
            await _eventProcessor.ProcessAsync(challenge.Events);
        }
    }

    #endregion

    #region Multi-Factor Authentication Management

    public async Task UpdateRequiredMethodsAsync(UserId userId, int requiredMethods, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating required methods for user: {UserId} to {RequiredMethods}", userId, requiredMethods);

        var multiFactorAuth = await _multiFactorAuthRepository.GetByUserIdAsync(userId, cancellationToken);
        if (multiFactorAuth is null)
        {
            throw new MultiFactorAuthNotFoundException(userId);
        }

        multiFactorAuth.UpdateRequiredMethods(requiredMethods);
        await _multiFactorAuthRepository.UpdateAsync(multiFactorAuth, cancellationToken);
        await _eventProcessor.ProcessAsync(multiFactorAuth.Events);
    }

    public async Task ActivateMultiFactorAuthAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Activating MFA for user: {UserId}", userId);

        var multiFactorAuth = await _multiFactorAuthRepository.GetByUserIdAsync(userId, cancellationToken);
        if (multiFactorAuth is null)
        {
            throw new MultiFactorAuthNotFoundException(userId);
        }

        if (multiFactorAuth.Status == MultiFactorAuthStatus.Active)
        {
            throw new MultiFactorAlreadyActiveException(userId);
        }

        multiFactorAuth.Activate();
        await _multiFactorAuthRepository.UpdateAsync(multiFactorAuth, cancellationToken);
        await _eventProcessor.ProcessAsync(multiFactorAuth.Events);

        await _messageBroker.PublishAsync(new Mamey.Government.Identity.Application.Events.UserMultiFactorEnabled(userId, multiFactorAuth.EnabledMethods, multiFactorAuth.RequiredMethods, _clock.CurrentDate()));
    }

    public async Task DeactivateMultiFactorAuthAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deactivating MFA for user: {UserId}", userId);

        var multiFactorAuth = await _multiFactorAuthRepository.GetByUserIdAsync(userId, cancellationToken);
        if (multiFactorAuth is null)
        {
            throw new MultiFactorAuthNotFoundException(userId);
        }

        if (multiFactorAuth.Status == MultiFactorAuthStatus.Inactive)
        {
            throw new MultiFactorAlreadyInactiveException(userId);
        }

        multiFactorAuth.Deactivate();
        await _multiFactorAuthRepository.UpdateAsync(multiFactorAuth, cancellationToken);
        await _eventProcessor.ProcessAsync(multiFactorAuth.Events);

        await _messageBroker.PublishAsync(new Mamey.Government.Identity.Application.Events.UserMultiFactorDisabled(userId, _clock.CurrentDate()));
    }

    #endregion

    #region Multi-Factor Authentication Statistics

    public async Task<MultiFactorAuthStatisticsDto> GetMultiFactorAuthStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var totalMFA = await _multiFactorAuthRepository.CountAsync(cancellationToken);
        var activeMFA = await _multiFactorAuthRepository.CountByStatusAsync(MultiFactorAuthStatus.Active, cancellationToken);
        var inactiveMFA = await _multiFactorAuthRepository.CountByStatusAsync(MultiFactorAuthStatus.Inactive, cancellationToken);
        var totalChallenges = await _mfaChallengeRepository.CountAsync(cancellationToken);
        var pendingChallenges = await _mfaChallengeRepository.CountByStatusAsync(MfaChallengeStatus.Pending, cancellationToken);

        return new MultiFactorAuthStatisticsDto
        {
            TotalMultiFactorAuth = totalMFA,
            ActiveMultiFactorAuth = activeMFA,
            InactiveMultiFactorAuth = inactiveMFA,
            TotalChallenges = totalChallenges,
            PendingChallenges = pendingChallenges
        };
    }

    #endregion

    #region Private Helper Methods

    private static MultiFactorAuthDto MapToMultiFactorAuthDto(MultiFactorAuth multiFactorAuth)
    {
        return new MultiFactorAuthDto(
            multiFactorAuth.Id,
            multiFactorAuth.UserId,
            multiFactorAuth.EnabledMethods.Select(m => m.ToString()),
            multiFactorAuth.RequiredMethods,
            multiFactorAuth.Status.ToString(),
            multiFactorAuth.CreatedAt,
            multiFactorAuth.ActivatedAt
        );
    }

    private static MfaChallengeDto MapToMfaChallengeDto(MfaChallenge challenge)
    {
        return new MfaChallengeDto(
            challenge.Id,
            challenge.MultiFactorAuthId,
            challenge.Method.ToString(),
            challenge.ChallengeData,
            challenge.ExpiresAt,
            challenge.IpAddress,
            challenge.UserAgent,
            challenge.Status.ToString(),
            challenge.CreatedAt,
            challenge.VerifiedAt
        );
    }

    #endregion
}

