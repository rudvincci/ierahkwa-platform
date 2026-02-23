using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Compliance.Domain.Entities;

namespace Pupitre.Compliance.Application.Exceptions;

internal class ComplianceRecordNotFoundException : MameyException
{
    public ComplianceRecordNotFoundException(ComplianceRecordId compliancerecordId)
        : base($"ComplianceRecord with ID: '{compliancerecordId.Value}' was not found.")
        => ComplianceRecordId = compliancerecordId;

    public ComplianceRecordId ComplianceRecordId { get; }
}

