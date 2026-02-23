using Mamey.Exceptions;
using Mamey.Government.Identity.Domain.Exceptions;

namespace Mamey.Government.Identity.Application.Exceptions;

public class TwoFactorAuthAlreadyExistsException : DomainException
{
    public TwoFactorAuthAlreadyExistsException(Guid twoFactorAuthId) : base($"Two-factor authentication with ID '{twoFactorAuthId}' already exists.")
    {
    }
}
