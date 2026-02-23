using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Saga.Api.Messages.Citizenship;

/// <summary>
/// Saga message for passport issuance.
/// </summary>
internal record PassportIssued(
    Guid PassportId, 
    Guid CitizenId,
    Guid ApplicationId,
    Guid TenantId,
    string PassportNumber,
    DateTimeOffset? IssuedAt = null) : IEvent;
