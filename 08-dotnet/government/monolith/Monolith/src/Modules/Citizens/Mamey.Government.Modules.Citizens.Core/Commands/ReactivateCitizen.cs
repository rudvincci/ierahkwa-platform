using System;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;

namespace Mamey.Government.Modules.Citizens.Core.Commands;

/// <summary>
/// Reactivates a suspended citizen, optionally setting a specific status.
/// If no target status is provided, the citizen will be restored to their previous status before suspension.
/// </summary>
public record ReactivateCitizen(
    Guid CitizenId,
    string Reason,
    string ReactivatedBy,
    CitizenshipStatus? TargetStatus = null) : ICommand;
