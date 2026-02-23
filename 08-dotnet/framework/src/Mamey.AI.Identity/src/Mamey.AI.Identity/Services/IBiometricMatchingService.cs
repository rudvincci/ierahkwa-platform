namespace Mamey.AI.Identity.Services;

/// <summary>
/// Service for biometric matching operations (face, fingerprint, voice).
/// </summary>
public interface IBiometricMatchingService
{
    /// <summary>
    /// Compares two face images and returns a similarity score (0-1).
    /// </summary>
    Task<double> CompareFacesAsync(
        Stream referenceImage,
        Stream probeImage,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Compares two fingerprint templates and returns a similarity score (0-1).
    /// </summary>
    Task<double> CompareFingerprintsAsync(
        byte[] referenceTemplate,
        byte[] probeTemplate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Compares two voice samples and returns a similarity score (0-1).
    /// </summary>
    Task<double> CompareVoicesAsync(
        Stream referenceAudio,
        Stream probeAudio,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Detects liveness in a video stream to prevent spoofing.
    /// </summary>
    Task<bool> DetectLivenessAsync(
        Stream videoStream,
        CancellationToken cancellationToken = default);
}
