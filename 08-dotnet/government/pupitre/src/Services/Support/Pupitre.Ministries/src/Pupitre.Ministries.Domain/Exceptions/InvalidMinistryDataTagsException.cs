using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.Ministries.Domain.Exceptions;

internal class InvalidMinistryDataTagsException : DomainException
{
    public override string Code { get; } = "invalid_ministrydata_tags";

    public InvalidMinistryDataTagsException() : base("MinistryData tags are invalid.")
    {
    }
}
