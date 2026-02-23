using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MissingConfirmationCodeException : DomainException
{
    public override string Code { get; } = "missing_confirmation_code";

    public MissingConfirmationCodeException() : base("Confirmation code is missing.")
    {
    }
}
