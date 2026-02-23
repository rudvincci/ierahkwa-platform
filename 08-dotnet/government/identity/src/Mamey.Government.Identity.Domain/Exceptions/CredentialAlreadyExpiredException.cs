using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class CredentialAlreadyExpiredException : DomainException
{
    public override string Code { get; } = "credential_already_expired";

    public CredentialAlreadyExpiredException() : base("Credential is already expired.")
    {
    }
}
