using Mamey.CQRS.Commands;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.MessageBrokers;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Commands.Handlers;

/// <summary>
/// Handler for DID-based authentication command.
/// Delegates to DIDAuthenticationService and publishes events.
/// 
/// TDD Reference: Lines 1594-1703 (Identity Service)
/// BDD Reference: Lines 326-378 (V.2 Biometric Identity)
/// </summary>
internal sealed class AuthenticateWithDIDHandler : ICommandHandler<AuthenticateWithDID>
{
    private readonly IDIDAuthenticationService _didAuthService;
    private readonly IBusPublisher _busPublisher;
    private readonly ILogger<AuthenticateWithDIDHandler> _logger;

    public AuthenticateWithDIDHandler(
        IDIDAuthenticationService didAuthService,
        IBusPublisher busPublisher,
        ILogger<AuthenticateWithDIDHandler> logger)
    {
        _didAuthService = didAuthService ?? throw new ArgumentNullException(nameof(didAuthService));
        _busPublisher = busPublisher ?? throw new ArgumentNullException(nameof(busPublisher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(AuthenticateWithDID command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Handling AuthenticateWithDID: DID={DID}, ChallengeId={ChallengeId}",
            command.DID, command.ChallengeId);

        var result = await _didAuthService.AuthenticateAsync(command, cancellationToken);

        if (result.Success && result.IdentityId.HasValue)
        {
            // Publish success event
            await _busPublisher.PublishAsync(new DIDAuthenticationSucceeded(
                result.IdentityId.Value,
                result.DID,
                ExtractDIDMethod(result.DID),
                DateTime.UtcNow
            ));

            _logger.LogInformation(
                "DID authentication succeeded: DID={DID}, IdentityId={IdentityId}",
                result.DID, result.IdentityId);
        }
        else
        {
            // Publish failure event
            await _busPublisher.PublishAsync(new DIDAuthenticationFailed(
                command.DID,
                result.ErrorMessage ?? "Unknown error",
                result.ErrorCode,
                DateTime.UtcNow
            ));

            _logger.LogWarning(
                "DID authentication failed: DID={DID}, ErrorCode={ErrorCode}, Error={Error}",
                command.DID, result.ErrorCode, result.ErrorMessage);
        }
    }

    private static string ExtractDIDMethod(string did)
    {
        var parts = did.Split(':');
        return parts.Length >= 2 ? parts[1] : "unknown";
    }
}
