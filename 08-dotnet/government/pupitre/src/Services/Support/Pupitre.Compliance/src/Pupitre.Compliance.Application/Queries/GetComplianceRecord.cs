using System;
using Mamey.CQRS.Queries;
using Pupitre.Compliance.Application.DTO;

namespace Pupitre.Compliance.Application.Queries;

internal record GetComplianceRecord(Guid Id) : IQuery<ComplianceRecordDetailsDto>;
