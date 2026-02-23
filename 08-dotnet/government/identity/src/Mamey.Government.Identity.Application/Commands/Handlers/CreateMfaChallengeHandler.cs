using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Commands.Handlers;

internal sealed class CreateMfaChallengeHandler : ICommandHandler<CreateMfaChallenge>
{
    private readonly IMfaChallengeRepository _mfaChallengeRepository;
    private readonly IEventProcessor _eventProcessor;

    public CreateMfaChallengeHandler(IMfaChallengeRepository mfaChallengeRepository, IEventProcessor eventProcessor)
    {
        _mfaChallengeRepository = mfaChallengeRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(CreateMfaChallenge command, CancellationToken cancellationToken = default)
    {
        var mfaChallenge = await _mfaChallengeRepository.GetAsync(command.Id);
        
        if (mfaChallenge is not null)
        {
            throw new MfaChallengeAlreadyExistsException(command.Id);
        }

        mfaChallenge = MfaChallenge.Create(command.Id, command.MultiFactorAuthId, (MfaMethod)command.Method, command.ChallengeData, command.ExpiresAt, command.IpAddress, command.UserAgent);
        await _mfaChallengeRepository.AddAsync(mfaChallenge, cancellationToken);
        await _eventProcessor.ProcessAsync(mfaChallenge.Events);
    }
}
