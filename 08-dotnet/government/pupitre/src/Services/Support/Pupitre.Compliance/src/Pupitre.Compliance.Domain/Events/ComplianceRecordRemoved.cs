using Mamey.CQRS;
using Pupitre.Compliance.Domain.Entities;

namespace Pupitre.Compliance.Domain.Events;

internal record ComplianceRecordRemoved(ComplianceRecord ComplianceRecord) : IDomainEvent;