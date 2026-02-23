using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Exceptions;

internal class InvalidIdentityTagsException : DomainException
{
    public override string Code { get; } = "invalid_identity_tags";

    public InvalidIdentityTagsException() : base("Identity tags are invalid.")
    {
    }
}
