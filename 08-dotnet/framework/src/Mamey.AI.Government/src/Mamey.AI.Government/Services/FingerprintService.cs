using Microsoft.Extensions.Logging;

namespace Mamey.AI.Government.Services;

public class FingerprintService
{
    private readonly ILogger<FingerprintService> _logger;

    public FingerprintService(ILogger<FingerprintService> logger)
    {
        _logger = logger;
    }

    public async Task<double> MatchFingerprintsAsync(byte[] template1, byte[] template2, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Matching fingerprints...");
        await Task.Delay(50, cancellationToken);

        // Mock: Minutiae matching
        return 0.95;
    }
}
