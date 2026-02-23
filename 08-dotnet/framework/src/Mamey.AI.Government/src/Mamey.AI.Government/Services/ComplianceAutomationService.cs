using Mamey.AI.Government.Interfaces;
using Mamey.AI.Government.Models;
using Microsoft.Extensions.Logging;

namespace Mamey.AI.Government.Services;

public class ComplianceAutomationService : IComplianceService
{
    private readonly PolicyEngine _policyEngine;
    private readonly ILogger<ComplianceAutomationService> _logger;

    public ComplianceAutomationService(
        PolicyEngine policyEngine,
        ILogger<ComplianceAutomationService> logger)
    {
        _policyEngine = policyEngine;
        _logger = logger;
    }

    public async Task<bool> CheckComplianceAsync(object entityData, string regulationId, CancellationToken cancellationToken = default)
    {
        var result = await _policyEngine.EvaluateAsync(entityData, regulationId, cancellationToken);
        return result.IsCompliant;
    }

    public async Task<IEnumerable<string>> GetViolationsAsync(object entityData, CancellationToken cancellationToken = default)
    {
        // Evaluate against all known regulations or a default set
        var regulations = new[] { "GOV-005", "2025-ID01", "2025-DS01", "2025-AM01" };
        var allViolations = new List<string>();

        foreach (var reg in regulations)
        {
            var result = await _policyEngine.EvaluateAsync(entityData, reg, cancellationToken);
            if (!result.IsCompliant)
            {
                allViolations.AddRange(result.Violations);
            }
        }

        return allViolations;
    }
    
    // Additional method for full result
    public async Task<ComplianceResult> EvaluateFullComplianceAsync(object entityData, string regulationId, CancellationToken cancellationToken = default)
    {
        return await _policyEngine.EvaluateAsync(entityData, regulationId, cancellationToken);
    }
}
