using System.Linq;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Types;

namespace Mamey.Government.Identity.Application.Commands.Handlers;

internal sealed class CreateSessionHandler : ICommandHandler<CreateSession>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IEventProcessor _eventProcessor;

    public CreateSessionHandler(ISessionRepository sessionRepository, IEventProcessor eventProcessor)
    {
        _sessionRepository = sessionRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(CreateSession command, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetAsync(command.Id);
        
        if (session is not null)
        {
            throw new SessionAlreadyExistsException(command.Id);
        }

        // Check if access token already exists (shouldn't happen with proper JWT generation, but handle it)
        // This protects against old sessions with duplicate tokens from the previous bug
        var tokenExists = await _sessionRepository.AccessTokenExistsAsync(command.AccessToken, cancellationToken);
        if (tokenExists)
        {
            try
            {
                var existingSession = await _sessionRepository.GetByAccessTokenAsync(command.AccessToken, cancellationToken);
                if (existingSession is not null)
                {
                    // If the existing session is expired or not active, delete it and proceed
                    if (existingSession.ExpiresAt <= DateTime.UtcNow || existingSession.Status != SessionStatus.Active)
                    {
                        await _sessionRepository.DeleteAsync(existingSession.Id, cancellationToken);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Access token already exists for an active session. Session ID: {existingSession.Id}");
                    }
                }
            }
            catch (InvalidOperationException)
            {
                // Multiple sessions with the same token (from old bug) - this shouldn't happen but handle it
                // Delete all expired/revoked sessions with this token
                var expiredSessions = await _sessionRepository.GetExpiredSessionsAsync(cancellationToken);
                var sessionsToDelete = expiredSessions.Where(s => s?.AccessToken == command.AccessToken && s.Status != SessionStatus.Active).ToList();
                foreach (var expiredSession in sessionsToDelete)
                {
                    if (expiredSession != null)
                    {
                        await _sessionRepository.DeleteAsync(expiredSession.Id, cancellationToken);
                    }
                }
                // If there are still active sessions with this token, throw an error
                tokenExists = await _sessionRepository.AccessTokenExistsAsync(command.AccessToken, cancellationToken);
                if (tokenExists)
                {
                    throw new InvalidOperationException($"Cannot create session: access token collision with existing active session(s)");
                }
            }
        }

        session = Session.Create(command.Id, new UserId(command.UserId), command.AccessToken, command.RefreshToken, command.ExpiresAt, command.IpAddress, command.UserAgent);
        await _sessionRepository.AddAsync(session, cancellationToken);
        await _eventProcessor.ProcessAsync(session.Events);
    }
}
