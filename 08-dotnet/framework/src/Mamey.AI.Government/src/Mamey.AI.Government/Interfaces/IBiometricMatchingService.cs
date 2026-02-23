namespace Mamey.AI.Government.Interfaces;

public interface IBiometricMatchingService
{
    Task<double> CompareFacesAsync(Stream referenceImage, Stream probeImage, CancellationToken cancellationToken = default);
    Task<bool> DetectLivenessAsync(Stream videoStream, CancellationToken cancellationToken = default);
}
