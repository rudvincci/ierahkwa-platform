namespace Mamey.FWID.Identities.Domain.ValueObjects;

/// <summary>
/// Represents the quality assessment of biometric data.
/// Compliant with Biometric Verification Microservice spec (§2.2).
/// </summary>
public enum BiometricQuality
{
    /// <summary>
    /// Quality is unknown or not assessed.
    /// </summary>
    Unknown = 0,
    
    /// <summary>
    /// Good quality - meets all quality gates.
    /// </summary>
    Good = 1,
    
    /// <summary>
    /// Fair quality - meets minimum requirements but may have minor issues.
    /// </summary>
    Fair = 2,
    
    /// <summary>
    /// Poor quality - does not meet minimum requirements.
    /// </summary>
    Poor = 3
}

