using FluentEmail.Core;
using Mamey.Emails;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.Security;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Services;

/// <summary>
/// Service for handling email confirmation operations.
/// </summary>
internal sealed class EmailConfirmationService : IEmailConfirmationService
{
    private readonly IIdentityRepository _identityRepository;
    private readonly IEmailConfirmationRepository _emailConfirmationRepository;
    private readonly IFluentEmail _email;
    private readonly EmailOptions _emailOptions;
    private readonly ISecurityProvider _securityProvider;
    private readonly ILogger<EmailConfirmationService> _logger;

    public EmailConfirmationService(
        IIdentityRepository identityRepository,
        IEmailConfirmationRepository emailConfirmationRepository,
        IFluentEmail email,
        EmailOptions emailOptions,
        ISecurityProvider securityProvider,
        ILogger<EmailConfirmationService> logger)
    {
        _identityRepository = identityRepository ?? throw new ArgumentNullException(nameof(identityRepository));
        _emailConfirmationRepository = emailConfirmationRepository ?? throw new ArgumentNullException(nameof(emailConfirmationRepository));
        _email = email ?? throw new ArgumentNullException(nameof(email));
        _emailOptions = emailOptions ?? throw new ArgumentNullException(nameof(emailOptions));
        _securityProvider = securityProvider ?? throw new ArgumentNullException(nameof(securityProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<EmailConfirmationResult> CreateEmailConfirmationAsync(
        IdentityId identityId,
        string email,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));

        var identity = await _identityRepository.GetAsync(identityId, cancellationToken);
        if (identity == null)
            throw new InvalidOperationException("Identity not found");

        // Generate confirmation token
        var token = _securityProvider.GenerateRandomString(64, removeSpecialChars: true);
        var expiresAt = DateTime.UtcNow.AddDays(7); // 7 day expiration

        var confirmationId = new EmailConfirmationId();
        var confirmation = new EmailConfirmation(
            confirmationId,
            identityId,
            email,
            token,
            expiresAt);

        await _emailConfirmationRepository.AddAsync(confirmation, cancellationToken);

        // Send confirmation email
        await SendConfirmationEmailAsync(email, token, cancellationToken);

        _logger.LogInformation("Email confirmation created for identity {IdentityId}, email {Email}", identityId, email);

        return new EmailConfirmationResult
        {
            EmailConfirmationId = confirmationId,
            Token = token,
            ExpiresAt = expiresAt
        };
    }

    public async Task ConfirmEmailAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or empty", nameof(token));

        var confirmation = await _emailConfirmationRepository.GetByTokenAsync(token, cancellationToken);
        if (confirmation == null)
            throw new InvalidOperationException("Invalid confirmation token");

        confirmation.Confirm();
        await _emailConfirmationRepository.UpdateAsync(confirmation, cancellationToken);

        // Update identity email confirmed status
        var identity = await _identityRepository.GetAsync(confirmation.IdentityId, cancellationToken);
        if (identity != null)
        {
            identity.ConfirmEmail();
            await _identityRepository.UpdateAsync(identity, cancellationToken);
        }

        _logger.LogInformation("Email confirmed for identity {IdentityId}", confirmation.IdentityId);
    }

    public async Task ResendEmailConfirmationAsync(
        IdentityId identityId,
        string email,
        CancellationToken cancellationToken = default)
    {
        // Invalidate existing confirmations for this identity and email
        var existingConfirmations = await _emailConfirmationRepository.GetByIdentityIdAsync(identityId, cancellationToken);
        foreach (var existing in existingConfirmations.Where(c => c.Email == email && c.Status == Domain.ValueObjects.ConfirmationStatus.Pending))
        {
            existing.Expire();
            await _emailConfirmationRepository.UpdateAsync(existing, cancellationToken);
        }

        // Create new confirmation
        await CreateEmailConfirmationAsync(identityId, email, cancellationToken);

        _logger.LogInformation("Email confirmation resent for identity {IdentityId}, email {Email}", identityId, email);
    }

    public async Task<bool> IsEmailConfirmedAsync(
        IdentityId identityId,
        CancellationToken cancellationToken = default)
    {
        var identity = await _identityRepository.GetAsync(identityId, cancellationToken);
        return identity?.EmailConfirmed ?? false;
    }

    public async Task<int> CleanupExpiredEmailConfirmationsAsync(
        CancellationToken cancellationToken = default)
    {
        var expiredConfirmations = await _emailConfirmationRepository.FindAsync(
            c => c.Status == Domain.ValueObjects.ConfirmationStatus.Expired ||
                 c.ExpiresAt < DateTime.UtcNow,
            cancellationToken);

        int count = 0;
        foreach (var confirmation in expiredConfirmations)
        {
            await _emailConfirmationRepository.DeleteAsync(confirmation.Id, cancellationToken);
            count++;
        }

        _logger.LogInformation("Cleaned up {Count} expired email confirmations", count);

        return count;
    }

    private async Task SendConfirmationEmailAsync(string email, string token, CancellationToken cancellationToken)
    {
        try
        {
            var confirmationUrl = $"https://auth.futurewampum.com/confirm-email?token={token}";
            var subject = "Confirm Your Email Address";
            var body = $@"
                <html>
                <body>
                    <h2>Confirm Your Email Address</h2>
                    <p>Please click the link below to confirm your email address:</p>
                    <p><a href=""{confirmationUrl}"">{confirmationUrl}</a></p>
                    <p>This link will expire in 7 days.</p>
                    <p>If you did not request this confirmation, please ignore this email.</p>
                </body>
                </html>";

            // Use IFluentEmail directly, following the pattern from Notifications service
            var result = await _email
                .SetFrom(_emailOptions.EmailId, _emailOptions.Name)
                .To(email)
                .Subject(subject)
                .Body(body, isHtml: true)
                .SendAsync();

            if (!result.Successful)
            {
                _logger.LogError("Failed to send confirmation email to {Email}.\n{Errors}",
                    email, string.Join(Environment.NewLine, result.ErrorMessages));
            }
            else
            {
                _logger.LogInformation("Confirmation email sent to {Email}", email);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending confirmation email to {Email}", email);
            throw;
        }
    }

}

