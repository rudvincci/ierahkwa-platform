using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class UserNotLockedException : DomainException
{
    public override string Code { get; } = "user_not_locked";

    public UserNotLockedException() : base("User is not locked.")
    {
    }
}
