namespace Mamey.FWID.Identities.Domain.ValueObjects;

/// <summary>
/// Represents the type of biometric data.
/// </summary>
public enum BiometricType
{
    /// <summary>
    /// Facial recognition biometric.
    /// </summary>
    Facial = 1,
    
    /// <summary>
    /// Fingerprint biometric.
    /// </summary>
    Fingerprint = 2,
    
    /// <summary>
    /// Voice recognition biometric.
    /// </summary>
    Voice = 3,
    
    /// <summary>
    /// Iris recognition biometric.
    /// </summary>
    Iris = 4
}

