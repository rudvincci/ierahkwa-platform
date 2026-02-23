using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Passports.Core.Events;

/// <summary>
/// Integration event published when a passport is reported lost or stolen.
/// </summary>
public record PassportLostStolenEvent(
    Guid PassportId, 
    Guid CitizenId, 
    string ReportType,
    string Description) : IEvent;
