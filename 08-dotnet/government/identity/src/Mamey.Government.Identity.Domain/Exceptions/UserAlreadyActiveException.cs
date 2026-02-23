using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class UserAlreadyActiveException : DomainException
{
    public override string Code { get; } = "user_already_active";

    public UserAlreadyActiveException() : base("User is already active.")
    {
    }
}
