using System;

namespace Mamey.FWID.Sagas.Core.Data;

/// <summary>
/// Saga data structure for tracking Access Control Granting process.
/// Orchestrates: Identity Verification → Zone Validation → Zone Access Granting → Ledger Logging
/// </summary>
internal class AccessControlGrantingSagaData
{
    public Guid IdentityId { get; set; }
    public Guid ZoneId { get; set; }
    public string Permission { get; set; } = string.Empty;
    public bool IdentityVerified { get; set; }
    public bool ZoneValidated { get; set; }
    public bool AccessGranted { get; set; }
    public bool LedgerLogged { get; set; }
    public string Status { get; set; } = "Pending";
    public Guid? AccessControlId { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}



