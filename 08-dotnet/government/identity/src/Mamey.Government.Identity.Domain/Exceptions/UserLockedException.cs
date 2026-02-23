using Mamey.Exceptions;

namespace Mamey.Government.Identity.Domain.Exceptions;

public class UserLockedException : DomainException
{
    public UserLockedException() : base("User account is locked.")
    {
    }

    public UserLockedException(Guid userId) : base($"User account with ID '{userId}' is locked.")
    {
    }
}
