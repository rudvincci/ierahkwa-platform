using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Saga.Api.Messages.Citizenship;

/// <summary>
/// Saga message for travel identity issuance.
/// </summary>
internal record TravelIdentityIssued(
    Guid TravelIdentityId,
    Guid CitizenId,
    Guid ApplicationId,
    Guid TenantId,
    string TravelIdentityNumber,
    DateTimeOffset? IssuedAt = null) : IEvent;
