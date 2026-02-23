using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class CredentialNotActiveException : DomainException
{
    public override string Code { get; } = "credential_not_active";

    public CredentialNotActiveException() : base("Credential is not active.")
    {
    }
}
