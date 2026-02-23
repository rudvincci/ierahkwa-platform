using System.Runtime.CompilerServices;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.Commands;

[Contract]
public record VerifyMfaChallenge : ICommand
{
    public VerifyMfaChallenge(Guid challengeId, string response)
    {
        ChallengeId = challengeId;
        Response = response;
    }

    public Guid ChallengeId { get; init; }
    public string Response { get; init; }
}
