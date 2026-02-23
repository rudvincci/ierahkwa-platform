using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Events.Integration.Auth;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.MessageBrokers;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Commands.Handlers;

internal sealed class RefreshTokenHandler : ICommandHandler<RefreshToken>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IEventProcessor _eventProcessor;
    private readonly ISessionRepository _sessionRepository;
    private readonly IBusPublisher _busPublisher;
    private readonly ILogger<RefreshTokenHandler> _logger;

    public RefreshTokenHandler(
        IAuthenticationService authenticationService,
        IEventProcessor eventProcessor,
        ISessionRepository sessionRepository,
        IBusPublisher busPublisher,
        ILogger<RefreshTokenHandler> logger)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
        _busPublisher = busPublisher ?? throw new ArgumentNullException(nameof(busPublisher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(RefreshToken command, CancellationToken cancellationToken = default)
    {
        var result = await _authenticationService.RefreshTokenAsync(
            command.Token,
            cancellationToken);

        // Publish TokenRefreshed integration event
        try
        {
            var integrationEvent = new TokenRefreshedIntegrationEvent(
                result.IdentityId.Value,
                result.SessionId.Value,
                DateTime.UtcNow);

            await _busPublisher.PublishAsync(integrationEvent);
            _logger.LogInformation("Published TokenRefreshed integration event for IdentityId: {IdentityId}, SessionId: {SessionId}",
                result.IdentityId.Value, result.SessionId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing TokenRefreshed integration event for SessionId: {SessionId}", result.SessionId.Value);
            // Don't throw - token refresh succeeded, event publishing failure is non-critical
        }
    }
}

