using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class CredentialAlreadyInactiveException : DomainException
{
    public override string Code { get; } = "credential_already_inactive";

    public CredentialAlreadyInactiveException() : base("Credential is already inactive.")
    {
    }
}
