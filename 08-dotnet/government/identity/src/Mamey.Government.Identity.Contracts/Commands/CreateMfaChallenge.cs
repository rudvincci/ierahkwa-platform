using System.Runtime.CompilerServices;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.Commands;

[Contract]
public record CreateMfaChallenge : ICommand
{
    public CreateMfaChallenge(Guid id, Guid multiFactorAuthId, int method, string challengeData, DateTime expiresAt, string? ipAddress = null, string? userAgent = null)
    {
        Id = id;
        MultiFactorAuthId = multiFactorAuthId;
        Method = method;
        ChallengeData = challengeData;
        ExpiresAt = expiresAt;
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }

    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid MultiFactorAuthId { get; init; }
    public int Method { get; init; }
    public string ChallengeData { get; init; }
    public DateTime ExpiresAt { get; init; }
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
}
