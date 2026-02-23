using Mamey.Exceptions;
using Pupitre.Users.Domain.Entities;

namespace Pupitre.Users.Application.Exceptions;

internal class UserAlreadyExistsException : MameyException
{
    public UserAlreadyExistsException(UserId userId)
        : base($"User with ID: '{userId.Value}' already exists.")
        => UserId = userId;

    public UserId UserId { get; }
}
