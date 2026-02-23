using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class InvalidExpirationTimeException : DomainException
{
    public override string Code { get; } = "invalid_expiration_time";

    public InvalidExpirationTimeException() : base("Expiration time is invalid.")
    {
    }
}
