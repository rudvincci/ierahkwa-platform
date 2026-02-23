using Mamey.Exceptions;
using Mamey.Government.Identity.Domain.Exceptions;

namespace Mamey.Government.Identity.Application.Exceptions;

public class TwoFactorAuthNotFoundException : DomainException
{
    public TwoFactorAuthNotFoundException(Guid userId) : base($"Two-factor authentication for user '{userId}' was not found.")
    {
    }
}
