using Mamey.Exceptions;

namespace Pupitre.Aftercare.Domain.Exceptions;

internal class MissingAftercarePlanTagsException : DomainException
{
    public MissingAftercarePlanTagsException()
        : base("AftercarePlan tags are missing.")
    {
    }

    public override string Code => "missing_aftercareplan_tags";
}