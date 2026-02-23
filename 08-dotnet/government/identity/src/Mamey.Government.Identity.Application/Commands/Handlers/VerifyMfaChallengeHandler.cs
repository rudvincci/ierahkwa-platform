using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Commands.Handlers;

internal sealed class VerifyMfaChallengeHandler : ICommandHandler<VerifyMfaChallenge>
{
    private readonly IMfaChallengeRepository _mfaChallengeRepository;
    private readonly IEventProcessor _eventProcessor;

    public VerifyMfaChallengeHandler(IMfaChallengeRepository mfaChallengeRepository, IEventProcessor eventProcessor)
    {
        _mfaChallengeRepository = mfaChallengeRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(VerifyMfaChallenge command, CancellationToken cancellationToken = default)
    {
        var mfaChallenge = await _mfaChallengeRepository.GetAsync(command.ChallengeId);
        
        if (mfaChallenge is null)
        {
            throw new MfaChallengeNotFoundException(command.ChallengeId);
        }

        mfaChallenge.Verify(command.Response);
        await _mfaChallengeRepository.UpdateAsync(mfaChallenge, cancellationToken);
        await _eventProcessor.ProcessAsync(mfaChallenge.Events);
    }
}
