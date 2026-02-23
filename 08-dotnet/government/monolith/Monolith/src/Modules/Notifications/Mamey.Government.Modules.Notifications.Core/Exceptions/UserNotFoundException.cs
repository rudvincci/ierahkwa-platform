using Mamey.Exceptions;

namespace Mamey.Government.Modules.Notifications.Core.Exceptions;

public class UserNotFoundException : MameyException
{
    public Guid UserId { get; }

    public UserNotFoundException(Guid userId) : base($"User with id '{userId} was not found.'")
        => UserId = userId;
}