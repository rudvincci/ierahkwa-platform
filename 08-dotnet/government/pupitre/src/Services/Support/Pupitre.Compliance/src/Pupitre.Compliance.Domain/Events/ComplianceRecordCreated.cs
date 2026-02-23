using Mamey.CQRS;
using Pupitre.Compliance.Domain.Entities;

namespace Pupitre.Compliance.Domain.Events;

internal record ComplianceRecordCreated(ComplianceRecord ComplianceRecord) : IDomainEvent;

