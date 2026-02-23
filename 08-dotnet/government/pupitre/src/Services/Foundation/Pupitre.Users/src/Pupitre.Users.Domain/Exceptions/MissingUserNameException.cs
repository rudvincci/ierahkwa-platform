using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.Users.Domain.Exceptions;

internal class MissingUserNameException : DomainException
{
    public MissingUserNameException()
        : base("User name is missing.")
    {
    }

    public override string Code => "missing_user_name";
}
