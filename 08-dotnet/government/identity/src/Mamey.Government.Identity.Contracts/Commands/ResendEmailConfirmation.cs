using System.Runtime.CompilerServices;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.Commands;

[Contract]
public record ResendEmailConfirmation : ICommand
{
    public ResendEmailConfirmation(Guid userId, string newConfirmationCode, DateTime newExpiresAt)
    {
        UserId = userId;
        NewConfirmationCode = newConfirmationCode;
        NewExpiresAt = newExpiresAt;
    }

    public Guid UserId { get; init; }
    public string NewConfirmationCode { get; init; }
    public DateTime NewExpiresAt { get; init; }
}
