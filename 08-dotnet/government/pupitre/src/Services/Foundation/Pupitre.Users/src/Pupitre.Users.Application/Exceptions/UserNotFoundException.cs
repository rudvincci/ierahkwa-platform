using Mamey.Exceptions;
using Pupitre.Users.Domain.Entities;

namespace Pupitre.Users.Application.Exceptions;

internal class UserNotFoundException : MameyException
{
    public UserNotFoundException(UserId userId)
        : base($"User with ID: '{userId.Value}' was not found.")
        => UserId = userId;

    public UserId UserId { get; }
}
