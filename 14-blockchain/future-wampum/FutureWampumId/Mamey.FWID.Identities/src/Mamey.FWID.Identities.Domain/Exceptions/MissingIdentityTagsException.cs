using Mamey.Exceptions;

namespace Mamey.FWID.Identities.Domain.Exceptions;

internal class MissingIdentityTagsException : DomainException
{
    public MissingIdentityTagsException()
        : base("Identity tags are missing.")
    {
    }

    public override string Code => "missing_identity_tags";
}