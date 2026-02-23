using Mamey.Exceptions;

namespace Pupitre.Compliance.Domain.Exceptions;

internal class MissingComplianceRecordTagsException : DomainException
{
    public MissingComplianceRecordTagsException()
        : base("ComplianceRecord tags are missing.")
    {
    }

    public override string Code => "missing_compliancerecord_tags";
}