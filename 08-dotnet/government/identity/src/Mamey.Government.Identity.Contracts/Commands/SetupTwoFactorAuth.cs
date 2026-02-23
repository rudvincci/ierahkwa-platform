using System.Runtime.CompilerServices;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.Commands;

[Contract]
public record SetupTwoFactorAuth : ICommand
{
    public SetupTwoFactorAuth(Guid id, Guid userId, string secretKey, string qrCodeUrl)
    {
        Id = id;
        UserId = userId;
        SecretKey = secretKey;
        QrCodeUrl = qrCodeUrl;
    }

    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid UserId { get; init; }
    public string SecretKey { get; init; }
    public string QrCodeUrl { get; init; }
}
