using System.Runtime.CompilerServices;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Application.Commands;

public record CreateSession : ICommand
{
    public CreateSession(Guid id, Guid userId, string accessToken, string refreshToken, DateTime expiresAt, string? ipAddress = null, string? userAgent = null)
    {
        Id = id;
        UserId = userId;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        ExpiresAt = expiresAt;
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }

    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid UserId { get; init; }
    public string AccessToken { get; init; }
    public string RefreshToken { get; init; }
    public DateTime ExpiresAt { get; init; }
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
}
