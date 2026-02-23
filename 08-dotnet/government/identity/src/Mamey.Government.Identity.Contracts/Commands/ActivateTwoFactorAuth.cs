using System.Runtime.CompilerServices;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.Commands;

[Contract]
public record ActivateTwoFactorAuth : ICommand
{
    public ActivateTwoFactorAuth(Guid userId, string totpCode)
    {
        UserId = userId;
        TotpCode = totpCode;
    }

    public Guid UserId { get; init; }
    public string TotpCode { get; init; }
}
