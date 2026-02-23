using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class GetActiveMfaChallengeHandler : IQueryHandler<GetActiveMfaChallenge, MfaChallengeDto>
{
    private readonly IMfaChallengeRepository _mfaChallengeRepository;

    public GetActiveMfaChallengeHandler(IMfaChallengeRepository mfaChallengeRepository)
    {
        _mfaChallengeRepository = mfaChallengeRepository;
    }

    public async Task<MfaChallengeDto> HandleAsync(GetActiveMfaChallenge query, CancellationToken cancellationToken = default)
    {
        var challenge = await _mfaChallengeRepository.GetActiveByUserIdAsync(query.UserId, cancellationToken);
        
        if (challenge is null)
        {
            throw new MfaChallengeNotFoundException(query.UserId);
        }

        return MapToMfaChallengeDto(challenge);
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
}
