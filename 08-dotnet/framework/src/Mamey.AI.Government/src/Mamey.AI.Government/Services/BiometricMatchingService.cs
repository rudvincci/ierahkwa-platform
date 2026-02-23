using Mamey.AI.Government.Interfaces;
using Microsoft.Extensions.Logging;

namespace Mamey.AI.Government.Services;

public class BiometricMatchingService : IBiometricMatchingService
{
    private readonly FaceMatchingService _faceService;
    private readonly LivenessDetectionService _livenessService;
    private readonly FingerprintService _fingerprintService;
    private readonly ILogger<BiometricMatchingService> _logger;

    public BiometricMatchingService(
        FaceMatchingService faceService,
        LivenessDetectionService livenessService,
        FingerprintService fingerprintService,
        ILogger<BiometricMatchingService> logger)
    {
        _faceService = faceService;
        _livenessService = livenessService;
        _fingerprintService = fingerprintService;
        _logger = logger;
    }

    public async Task<double> CompareFacesAsync(Stream referenceImage, Stream probeImage, CancellationToken cancellationToken = default)
    {
        return await _faceService.MatchFacesAsync(referenceImage, probeImage, cancellationToken);
    }

    public async Task<bool> DetectLivenessAsync(Stream videoStream, CancellationToken cancellationToken = default)
    {
        return await _livenessService.CheckLivenessAsync(videoStream, cancellationToken);
    }

    // Extension for fingerprint (not in interface yet, but available)
    public async Task<double> CompareFingerprintsAsync(byte[] t1, byte[] t2, CancellationToken cancellationToken = default)
    {
        return await _fingerprintService.MatchFingerprintsAsync(t1, t2, cancellationToken);
    }
}
