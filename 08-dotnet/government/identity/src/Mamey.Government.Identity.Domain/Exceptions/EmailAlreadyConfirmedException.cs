using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class EmailAlreadyConfirmedException : DomainException
{
    public override string Code { get; } = "email_already_confirmed";

    public EmailAlreadyConfirmedException() : base("Email is already confirmed.")
    {
    }
}
