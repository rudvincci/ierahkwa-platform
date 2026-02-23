using System;
using Mamey.Exceptions;

namespace Mamey.Government.Modules.Identity.Core.Exceptions;

public sealed class UserProfileNotFoundException : MameyException
{
    public Guid UserId { get; }

    public UserProfileNotFoundException(Guid userId)
        : base($"User profile with ID '{userId}' was not found.")
    {
        UserId = userId;
    }
}
