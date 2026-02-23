using Microsoft.Extensions.Logging;
using Mamey.AI.Government.Models;

namespace Mamey.AI.Government.Services;

public class SanctionsScreeningService
{
    private readonly ILogger<SanctionsScreeningService> _logger;

    public SanctionsScreeningService(ILogger<SanctionsScreeningService> logger)
    {
        _logger = logger;
    }

    public async Task<SanctionsCheckResult> CheckSanctionsAsync(string fullName, string nationality, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Performing sanctions screening for {Name}...", fullName);
        await Task.Delay(50, cancellationToken);

        // Mock logic: Check if name contains "Terror" or "Launder"
        if (fullName.Contains("Terror", StringComparison.OrdinalIgnoreCase))
        {
            return new SanctionsCheckResult 
            { 
                IsClean = false, 
                MatchedLists = new List<string> { "Mock Terror List" } 
            };
        }

        return new SanctionsCheckResult { IsClean = true };
    }
}
