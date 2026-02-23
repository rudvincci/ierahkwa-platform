using System.Runtime.CompilerServices;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.Commands;

[Contract]
public record ConfirmEmail : ICommand
{
    public ConfirmEmail(string confirmationCode)
    {
        ConfirmationCode = confirmationCode;
    }

    public string ConfirmationCode { get; init; }
}
