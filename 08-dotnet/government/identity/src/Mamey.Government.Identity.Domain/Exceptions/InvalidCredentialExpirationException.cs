using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class InvalidCredentialExpirationException : DomainException
{
    public override string Code { get; } = "invalid_credential_expiration";

    public InvalidCredentialExpirationException() : base("Credential expiration time is invalid.")
    {
    }
}
