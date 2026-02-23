using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.Compliance.Domain.Exceptions;

internal class MissingComplianceRecordNameException : DomainException
{
    public MissingComplianceRecordNameException()
        : base("ComplianceRecord name is missing.")
    {
    }

    public override string Code => "missing_compliancerecord_name";
}
