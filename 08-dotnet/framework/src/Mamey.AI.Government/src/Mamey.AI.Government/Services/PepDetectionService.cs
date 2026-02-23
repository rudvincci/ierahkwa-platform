using Microsoft.Extensions.Logging;
using Mamey.AI.Government.Models;

namespace Mamey.AI.Government.Services;

public class PepDetectionService
{
    private readonly ILogger<PepDetectionService> _logger;

    public PepDetectionService(ILogger<PepDetectionService> logger)
    {
        _logger = logger;
    }

    public async Task<PepCheckResult> CheckPepStatusAsync(string fullName, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking PEP status for {Name}...", fullName);
        await Task.Delay(50, cancellationToken);

        // Mock logic: Check if name contains "Minister" or "President"
        if (fullName.Contains("Minister", StringComparison.OrdinalIgnoreCase))
        {
            return new PepCheckResult
            {
                IsPep = true,
                Details = "Potential match in PEP database: Cabinet Minister"
            };
        }

        return new PepCheckResult { IsPep = false };
    }
}
