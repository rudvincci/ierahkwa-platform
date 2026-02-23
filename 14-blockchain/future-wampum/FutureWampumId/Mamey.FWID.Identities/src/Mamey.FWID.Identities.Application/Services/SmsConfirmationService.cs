using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.Security;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Services;

/// <summary>
/// Service for handling SMS confirmation operations.
/// </summary>
internal sealed class SmsConfirmationService : ISmsConfirmationService
{
    private readonly IIdentityRepository _identityRepository;
    private readonly ISmsConfirmationRepository _smsConfirmationRepository;
    private readonly ISecurityProvider _securityProvider;
    private readonly ILogger<SmsConfirmationService> _logger;
    private readonly ISmsService? _smsService; // Optional, can be null if not configured

    public SmsConfirmationService(
        IIdentityRepository identityRepository,
        ISmsConfirmationRepository smsConfirmationRepository,
        ISecurityProvider securityProvider,
        ILogger<SmsConfirmationService> logger,
        ISmsService? smsService = null)
    {
        _identityRepository = identityRepository ?? throw new ArgumentNullException(nameof(identityRepository));
        _smsConfirmationRepository = smsConfirmationRepository ?? throw new ArgumentNullException(nameof(smsConfirmationRepository));
        _securityProvider = securityProvider ?? throw new ArgumentNullException(nameof(securityProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _smsService = smsService;
    }

    public async Task<SmsConfirmationResult> CreateSmsConfirmationAsync(
        IdentityId identityId,
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be null or empty", nameof(phoneNumber));

        var identity = await _identityRepository.GetAsync(identityId, cancellationToken);
        if (identity == null)
            throw new InvalidOperationException("Identity not found");

        // Generate 6-digit confirmation code
        var code = GenerateNumericCode(6);
        var expiresAt = DateTime.UtcNow.AddMinutes(15); // 15 minute expiration

        var confirmationId = new SmsConfirmationId();
        var confirmation = new SmsConfirmation(
            confirmationId,
            identityId,
            phoneNumber,
            code,
            expiresAt);

        await _smsConfirmationRepository.AddAsync(confirmation, cancellationToken);

        // Send confirmation SMS
        await SendConfirmationSmsAsync(phoneNumber, code, cancellationToken);

        _logger.LogInformation("SMS confirmation created for identity {IdentityId}, phone {PhoneNumber}", identityId, phoneNumber);

        return new SmsConfirmationResult
        {
            SmsConfirmationId = confirmationId,
            Code = code,
            ExpiresAt = expiresAt
        };
    }

    public async Task ConfirmSmsAsync(
        IdentityId identityId,
        string code,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be null or empty", nameof(code));

        var confirmation = await _smsConfirmationRepository.GetByIdentityAndCodeAsync(identityId, code, cancellationToken);
        if (confirmation == null)
            throw new InvalidOperationException("Invalid confirmation code");

        try
        {
            confirmation.Confirm(code);
            await _smsConfirmationRepository.UpdateAsync(confirmation, cancellationToken);

            // Update identity phone confirmed status
            var identity = await _identityRepository.GetAsync(identityId, cancellationToken);
            if (identity != null)
            {
                identity.ConfirmPhone();
                await _identityRepository.UpdateAsync(identity, cancellationToken);
            }

            _logger.LogInformation("SMS confirmed for identity {IdentityId}", identityId);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("SMS confirmation failed for identity {IdentityId}: {Error}", identityId, ex.Message);
            throw;
        }
    }

    public async Task ResendSmsConfirmationAsync(
        IdentityId identityId,
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        // Invalidate existing confirmations for this identity and phone number
        var existingConfirmations = await _smsConfirmationRepository.GetByIdentityIdAsync(identityId, cancellationToken);
        foreach (var existing in existingConfirmations.Where(c => c.PhoneNumber == phoneNumber && c.Status == Domain.ValueObjects.ConfirmationStatus.Pending))
        {
            existing.Expire();
            await _smsConfirmationRepository.UpdateAsync(existing, cancellationToken);
        }

        // Create new confirmation
        await CreateSmsConfirmationAsync(identityId, phoneNumber, cancellationToken);

        _logger.LogInformation("SMS confirmation resent for identity {IdentityId}, phone {PhoneNumber}", identityId, phoneNumber);
    }

    public async Task<bool> IsPhoneConfirmedAsync(
        IdentityId identityId,
        CancellationToken cancellationToken = default)
    {
        var identity = await _identityRepository.GetAsync(identityId, cancellationToken);
        return identity?.PhoneConfirmed ?? false;
    }

    public async Task<int> CleanupExpiredSmsConfirmationsAsync(
        CancellationToken cancellationToken = default)
    {
        var expiredConfirmations = await _smsConfirmationRepository.FindAsync(
            c => c.Status == Domain.ValueObjects.ConfirmationStatus.Expired ||
                 c.ExpiresAt < DateTime.UtcNow,
            cancellationToken);

        int count = 0;
        foreach (var confirmation in expiredConfirmations)
        {
            await _smsConfirmationRepository.DeleteAsync(confirmation.Id, cancellationToken);
            count++;
        }

        _logger.LogInformation("Cleaned up {Count} expired SMS confirmations", count);

        return count;
    }

    private string GenerateNumericCode(int length)
    {
        var random = new Random();
        var code = "";
        for (int i = 0; i < length; i++)
        {
            code += random.Next(0, 10).ToString();
        }
        return code;
    }

    private async Task SendConfirmationSmsAsync(string phoneNumber, string code, CancellationToken cancellationToken)
    {
        try
        {
            var message = $"Your FutureWampum verification code is: {code}. This code expires in 15 minutes.";

            if (_smsService != null)
            {
                var success = await _smsService.SendSmsAsync(phoneNumber, message, cancellationToken);
                if (success)
                {
                    _logger.LogInformation("Confirmation SMS sent to {PhoneNumber}", phoneNumber);
                }
                else
                {
                    _logger.LogWarning("Failed to send confirmation SMS to {PhoneNumber}", phoneNumber);
                }
            }
            else
            {
                // Log the code if SMS service is not configured (for development/testing)
                _logger.LogWarning("SMS service not configured. Confirmation code for {PhoneNumber}: {Code}", phoneNumber, code);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending confirmation SMS to {PhoneNumber}", phoneNumber);
            throw;
        }
    }
}

/// <summary>
/// Interface for SMS service (to be implemented by Twilio or other SMS providers).
/// </summary>
public interface ISmsService
{
    Task<bool> SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
}

