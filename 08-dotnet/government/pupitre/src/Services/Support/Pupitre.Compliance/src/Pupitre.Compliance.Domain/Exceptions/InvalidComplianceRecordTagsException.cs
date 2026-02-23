using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.Compliance.Domain.Exceptions;

internal class InvalidComplianceRecordTagsException : DomainException
{
    public override string Code { get; } = "invalid_compliancerecord_tags";

    public InvalidComplianceRecordTagsException() : base("ComplianceRecord tags are invalid.")
    {
    }
}
