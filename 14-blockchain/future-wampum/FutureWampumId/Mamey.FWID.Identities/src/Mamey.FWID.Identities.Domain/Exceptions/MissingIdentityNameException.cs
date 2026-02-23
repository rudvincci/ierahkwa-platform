using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Exceptions;

internal class MissingIdentityNameException : DomainException
{
    public MissingIdentityNameException()
        : base("Identity name is missing.")
    {
    }

    public override string Code => "missing_identity_name";
}
