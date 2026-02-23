using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Compliance.Domain.Entities;

namespace Pupitre.Compliance.Application.Exceptions;

internal class ComplianceRecordAlreadyExistsException : MameyException
{
    public ComplianceRecordAlreadyExistsException(ComplianceRecordId compliancerecordId)
        : base($"ComplianceRecord with ID: '{compliancerecordId.Value}' already exists.")
        => ComplianceRecordId = compliancerecordId;

    public ComplianceRecordId ComplianceRecordId { get; }
}
