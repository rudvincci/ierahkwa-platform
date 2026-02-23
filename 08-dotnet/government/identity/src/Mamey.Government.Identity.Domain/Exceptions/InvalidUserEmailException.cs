using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class InvalidUserEmailException : DomainException
{
    public override string Code { get; } = "invalid_user_email";

    public InvalidUserEmailException() : base("User email is invalid.")
    {
    }
}
