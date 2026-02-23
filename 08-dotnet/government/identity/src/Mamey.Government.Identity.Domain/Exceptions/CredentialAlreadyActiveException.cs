using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class CredentialAlreadyActiveException : DomainException
{
    public override string Code { get; } = "credential_already_active";

    public CredentialAlreadyActiveException() : base("Credential is already active.")
    {
    }
}
