namespace Mamey.Portal.Citizenship.Application.Models;

/// <summary>
/// Citizenship status levels in the progression system
/// </summary>
public enum CitizenshipStatus
{
    /// <summary>
    /// Initial status granted upon application approval (first 2 years)
    /// </summary>
    Probationary = 1,

    /// <summary>
    /// Status after completing 2 years as Probationary (years 3-5)
    /// </summary>
    Resident = 2,

    /// <summary>
    /// Permanent status after completing 5 total years (after year 5)
    /// </summary>
    Citizen = 3
}


