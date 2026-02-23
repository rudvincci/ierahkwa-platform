using Mamey.Exceptions;
using Mamey.Government.Identity.Domain.Exceptions;

namespace Mamey.Government.Identity.Application.Exceptions;

public class MultiFactorAuthNotFoundException : DomainException
{
    public MultiFactorAuthNotFoundException(Guid userId) : base($"Multi-factor authentication for user '{userId}' was not found.")
    {
    }
}
