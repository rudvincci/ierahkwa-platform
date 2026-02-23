using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.Aftercare.Domain.Exceptions;

internal class InvalidAftercarePlanTagsException : DomainException
{
    public override string Code { get; } = "invalid_aftercareplan_tags";

    public InvalidAftercarePlanTagsException() : base("AftercarePlan tags are invalid.")
    {
    }
}
