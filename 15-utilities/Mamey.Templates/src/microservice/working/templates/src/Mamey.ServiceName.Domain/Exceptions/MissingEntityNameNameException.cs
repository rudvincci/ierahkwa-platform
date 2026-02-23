using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Mamey.ServiceName.Domain.Exceptions;

internal class MissingEntityNameNameException : DomainException
{
    public MissingEntityNameNameException()
        : base("EntityName name is missing.")
    {
    }

    public override string Code => "missing_entityname_name";
}
