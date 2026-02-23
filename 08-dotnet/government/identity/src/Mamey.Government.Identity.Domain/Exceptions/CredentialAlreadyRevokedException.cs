using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class CredentialAlreadyRevokedException : DomainException
{
    public override string Code { get; } = "credential_already_revoked";

    public CredentialAlreadyRevokedException() : base("Credential is already revoked.")
    {
    }
}
