using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Government.Identity.Application.Commands;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Application.Events;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Exceptions;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Time;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Application.Services;

internal sealed class EmailConfirmationService : IEmailConfirmationService
{
    #region Read-only Fields

    private readonly ILogger<EmailConfirmationService> _logger;
    private readonly IEmailConfirmationRepository _emailConfirmationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IClock _clock;
    private readonly IMessageBroker _messageBroker;
    private readonly IEventProcessor _eventProcessor;
    private readonly ICommandDispatcher _commandDispatcher;

    #endregion

    public EmailConfirmationService(
        IEmailConfirmationRepository emailConfirmationRepository,
        IUserRepository userRepository,
        IClock clock,
        IMessageBroker messageBroker,
        IEventProcessor eventProcessor,
        ICommandDispatcher commandDispatcher,
        ILogger<EmailConfirmationService> logger)
    {
        _emailConfirmationRepository = emailConfirmationRepository;
        _userRepository = userRepository;
        _clock = clock;
        _messageBroker = messageBroker;
        _eventProcessor = eventProcessor;
        _commandDispatcher = commandDispatcher;
        _logger = logger;
    }

    #region Email Confirmation Operations

    public async Task<string> CreateEmailConfirmationAsync(CreateEmailConfirmation command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating email confirmation for user: {UserId}", command.UserId);

        var user = await _userRepository.GetAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundException(command.UserId);
        }

        // Check if user already has a pending confirmation
        var existingConfirmation = await _emailConfirmationRepository.GetByUserIdAsync(command.UserId, cancellationToken);
        if (existingConfirmation is not null && existingConfirmation.Status == EmailConfirmationStatus.Pending)
        {
            throw new EmailConfirmationAlreadyExistsException(command.UserId);
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);

        return command.ConfirmationCode;
    }

    public async Task ConfirmEmailAsync(ConfirmEmail command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Confirming email with code: {Code}", command.ConfirmationCode);

        var emailConfirmation = await _emailConfirmationRepository.GetByConfirmationCodeAsync(command.ConfirmationCode, cancellationToken);
        if (emailConfirmation is null)
        {
            throw new EmailConfirmationNotFoundException(command.ConfirmationCode);
        }

        if (emailConfirmation.Status != EmailConfirmationStatus.Pending)
        {
            throw new EmailConfirmationAlreadyConfirmedException($"Email confirmation '{emailConfirmation.Id}' has already been confirmed.");
        }

        if (emailConfirmation.ExpiresAt < _clock.CurrentDate())
        {
            throw new EmailConfirmationExpiredException(emailConfirmation.Id);
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);

        await _messageBroker.PublishAsync(new UserEmailConfirmed(emailConfirmation.UserId, emailConfirmation.Email, _clock.CurrentDate()));
    }

    public async Task ResendEmailConfirmationAsync(ResendEmailConfirmation command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Resending email confirmation for user: {UserId}", command.UserId);

        var emailConfirmation = await _emailConfirmationRepository.GetByUserIdAsync(command.UserId, cancellationToken);
        if (emailConfirmation is null)
        {
            throw new EmailConfirmationNotFoundException(command.UserId);
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);
    }

    #endregion

    #region Email Confirmation Queries

    public async Task<EmailConfirmationDto?> GetEmailConfirmationAsync(EmailConfirmationId id, CancellationToken cancellationToken = default)
    {
        var emailConfirmation = await _emailConfirmationRepository.GetAsync(id, cancellationToken);
        return emailConfirmation is null ? null : MapToEmailConfirmationDto(emailConfirmation);
    }

    public async Task<EmailConfirmationDto?> GetEmailConfirmationByCodeAsync(string confirmationCode, CancellationToken cancellationToken = default)
    {
        var emailConfirmation = await _emailConfirmationRepository.GetByConfirmationCodeAsync(confirmationCode, cancellationToken);
        return emailConfirmation is null ? null : MapToEmailConfirmationDto(emailConfirmation);
    }

    public async Task<EmailConfirmationDto?> GetEmailConfirmationByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var emailConfirmation = await _emailConfirmationRepository.GetByUserIdAsync(userId, cancellationToken);
        return emailConfirmation is null ? null : MapToEmailConfirmationDto(emailConfirmation);
    }

    #endregion

    #region Email Confirmation Management

    public async Task<bool> IsEmailConfirmationValidAsync(string confirmationCode, CancellationToken cancellationToken = default)
    {
        var emailConfirmation = await _emailConfirmationRepository.GetByConfirmationCodeAsync(confirmationCode, cancellationToken);
        return emailConfirmation is not null && 
               emailConfirmation.Status == EmailConfirmationStatus.Pending && 
               emailConfirmation.ExpiresAt > _clock.CurrentDate();
    }

    public async Task CleanupExpiredEmailConfirmationsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Cleaning up expired email confirmations");

        var expiredConfirmations = await _emailConfirmationRepository.GetExpiredAsync(cancellationToken);
        
        foreach (var confirmation in expiredConfirmations)
        {
            confirmation.Expire();
            await _emailConfirmationRepository.UpdateAsync(confirmation, cancellationToken);
            await _eventProcessor.ProcessAsync(confirmation.Events);
        }
    }

    public async Task<IEnumerable<EmailConfirmationDto>> GetPendingEmailConfirmationsAsync(CancellationToken cancellationToken = default)
    {
        var confirmations = await _emailConfirmationRepository.GetPendingAsync(cancellationToken);
        return confirmations.Select(MapToEmailConfirmationDto);
    }

    #endregion

    #region Email Confirmation Statistics

    public async Task<EmailConfirmationStatisticsDto> GetEmailConfirmationStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var totalConfirmations = await _emailConfirmationRepository.CountAsync(cancellationToken);
        var pendingConfirmations = await _emailConfirmationRepository.CountByStatusAsync(EmailConfirmationStatus.Pending, cancellationToken);
        var confirmedConfirmations = await _emailConfirmationRepository.CountByStatusAsync(EmailConfirmationStatus.Confirmed, cancellationToken);
        var expiredConfirmations = await _emailConfirmationRepository.CountExpiredAsync(_clock.CurrentDate(), cancellationToken);

        return new EmailConfirmationStatisticsDto
        {
            TotalConfirmations = totalConfirmations,
            PendingConfirmations = pendingConfirmations,
            ConfirmedConfirmations = confirmedConfirmations,
            ExpiredConfirmations = expiredConfirmations
        };
    }

    #endregion

    #region Private Helper Methods

    private static EmailConfirmationDto MapToEmailConfirmationDto(EmailConfirmation emailConfirmation)
    {
        return new EmailConfirmationDto(
            emailConfirmation.Id,
            emailConfirmation.UserId,
            emailConfirmation.Email,
            emailConfirmation.ConfirmationCode,
            emailConfirmation.ExpiresAt,
            emailConfirmation.IpAddress,
            emailConfirmation.UserAgent,
            emailConfirmation.Status.ToString(),
            emailConfirmation.CreatedAt,
            emailConfirmation.ConfirmedAt
        );
    }

    #endregion
}

