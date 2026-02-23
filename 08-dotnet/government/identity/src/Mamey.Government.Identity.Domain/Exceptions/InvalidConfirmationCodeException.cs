using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class InvalidConfirmationCodeException : DomainException
{
    public override string Code { get; } = "invalid_confirmation_code";

    public InvalidConfirmationCodeException() : base("Confirmation code is invalid.")
    {
    }
}
