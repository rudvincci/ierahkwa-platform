using Mamey.Exceptions;

namespace Pupitre.Ministries.Domain.Exceptions;

internal class MissingMinistryDataTagsException : DomainException
{
    public MissingMinistryDataTagsException()
        : base("MinistryData tags are missing.")
    {
    }

    public override string Code => "missing_ministrydata_tags";
}