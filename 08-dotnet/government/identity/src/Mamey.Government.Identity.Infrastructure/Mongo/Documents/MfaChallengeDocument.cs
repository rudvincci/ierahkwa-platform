using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Infrastructure.Mongo.Documents;

internal class MfaChallengeDocument : IIdentifiable<Guid>
{
    public MfaChallengeDocument()
    {
    }

    public MfaChallengeDocument(MfaChallenge mfaChallenge)
    {
        if (mfaChallenge is null)
        {
            throw new NullReferenceException();
        }

        Id = mfaChallenge.Id.Value;
        MultiFactorAuthId = mfaChallenge.MultiFactorAuthId.Value;
        Method = mfaChallenge.Method.ToString();
        ChallengeData = mfaChallenge.ChallengeData;
        CreatedAt = mfaChallenge.CreatedAt.ToUnixTimeMilliseconds();
        ExpiresAt = mfaChallenge.ExpiresAt.ToUnixTimeMilliseconds();
        Status = mfaChallenge.Status.ToString();
        VerifiedAt = mfaChallenge.VerifiedAt?.ToUnixTimeMilliseconds();
        AttemptCount = mfaChallenge.AttemptCount;
        IpAddress = mfaChallenge.IpAddress;
        UserAgent = mfaChallenge.UserAgent;
        Version = mfaChallenge.Version;
    }

    public Guid Id { get; set; }
    public Guid MultiFactorAuthId { get; set; }
    public string Method { get; set; }
    public string ChallengeData { get; set; }
    public long CreatedAt { get; set; }
    public long ExpiresAt { get; set; }
    public string Status { get; set; }
    public long? VerifiedAt { get; set; }
    public int AttemptCount { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public int Version { get; set; }

    public MfaChallenge AsEntity()
        => new MfaChallenge(
            Id,
            MultiFactorAuthId,
            Method.ToEnum<MfaMethod>(),
            ChallengeData,
            CreatedAt.GetDate(),
            ExpiresAt.GetDate(),
            Status.ToEnum<MfaChallengeStatus>(),
            VerifiedAt?.GetDate(),
            IpAddress,
            UserAgent,
            Version);

    public MfaChallengeDto AsDto()
        => new MfaChallengeDto(
            Id,
            MultiFactorAuthId,
            Method,
            ChallengeData,
            ExpiresAt.GetDate(),
            IpAddress,
            UserAgent,
            Status,
            CreatedAt.GetDate(),
            VerifiedAt?.GetDate());
}

