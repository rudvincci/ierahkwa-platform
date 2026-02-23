using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class UserAlreadyInactiveException : DomainException
{
    public override string Code { get; } = "user_already_inactive";

    public UserAlreadyInactiveException() : base("User is already inactive.")
    {
    }
}
