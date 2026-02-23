using Mamey.Exceptions;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Exceptions;

public class UserAlreadyExistsException : MameyException
{
    public Guid UserId { get; }

    public UserAlreadyExistsException(Guid userId) : base($"User with id '{userId} already exists.'")
        => UserId = userId;
}