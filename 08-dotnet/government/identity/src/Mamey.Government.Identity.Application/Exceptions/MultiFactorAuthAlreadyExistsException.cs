using Mamey.Exceptions;
using Mamey.Government.Identity.Domain.Exceptions;

namespace Mamey.Government.Identity.Application.Exceptions;

public class MultiFactorAuthAlreadyExistsException : DomainException
{
    public MultiFactorAuthAlreadyExistsException(Guid multiFactorAuthId) : base($"Multi-factor authentication with ID '{multiFactorAuthId}' already exists.")
    {
    }
}
