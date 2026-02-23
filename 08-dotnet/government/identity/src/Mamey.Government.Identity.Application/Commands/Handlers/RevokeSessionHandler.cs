using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Commands.Handlers;

internal sealed class RevokeSessionHandler : ICommandHandler<RevokeSession>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IEventProcessor _eventProcessor;

    public RevokeSessionHandler(ISessionRepository sessionRepository, IEventProcessor eventProcessor)
    {
        _sessionRepository = sessionRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(RevokeSession command, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetAsync(command.SessionId);
        
        if (session is null)
        {
            throw new SessionNotFoundException(command.SessionId);
        }

        session.Revoke();
        await _sessionRepository.UpdateAsync(session, cancellationToken);
        await _eventProcessor.ProcessAsync(session.Events);
    }
}
