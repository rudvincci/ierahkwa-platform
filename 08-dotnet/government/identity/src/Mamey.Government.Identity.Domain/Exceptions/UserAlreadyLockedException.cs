using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class UserAlreadyLockedException : DomainException
{
    public override string Code { get; } = "user_already_locked";

    public UserAlreadyLockedException() : base("User is already locked.")
    {
    }
}
