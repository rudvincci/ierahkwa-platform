namespace Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;

/// <summary>
/// Citizenship status progression: Probationary (2 years) -> Resident (3 years) -> Citizen (permanent)
/// </summary>
public enum CitizenshipStatus
{
    Probationary = 0,  // Initial status after citizenship approval (2 years)
    Resident = 1,      // After 2 years probationary period (3 years)
    Citizen = 2,       // Permanent citizenship status
    Suspended = 50,    // Temporarily suspended citizenship
    Inactive = 99      // Deactivated/revoked citizenship
}
