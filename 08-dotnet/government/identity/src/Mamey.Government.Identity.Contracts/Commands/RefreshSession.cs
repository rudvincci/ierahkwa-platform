using System.Runtime.CompilerServices;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.Commands;

[Contract]
public record RefreshSession : ICommand
{
    public RefreshSession(Guid sessionId, string newAccessToken, string newRefreshToken, DateTime newExpiresAt)
    {
        SessionId = sessionId;
        NewAccessToken = newAccessToken;
        NewRefreshToken = newRefreshToken;
        NewExpiresAt = newExpiresAt;
    }

    public Guid SessionId { get; init; }
    public string NewAccessToken { get; init; }
    public string NewRefreshToken { get; init; }
    public DateTime NewExpiresAt { get; init; }
}
