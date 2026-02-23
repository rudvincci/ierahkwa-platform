using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.Ministries.Domain.Exceptions;

internal class MissingMinistryDataNameException : DomainException
{
    public MissingMinistryDataNameException()
        : base("MinistryData name is missing.")
    {
    }

    public override string Code => "missing_ministrydata_name";
}
