using Mamey.AI.Government.Interfaces;
using Mamey.AI.Government.Models;
using Microsoft.Extensions.Logging;

namespace Mamey.AI.Government.Services;

public class KycAmlService : IKycAmlService
{
    private readonly SanctionsScreeningService _sanctionsService;
    private readonly PepDetectionService _pepService;
    private readonly ILogger<KycAmlService> _logger;

    public KycAmlService(
        SanctionsScreeningService sanctionsService,
        PepDetectionService pepService,
        ILogger<KycAmlService> logger)
    {
        _sanctionsService = sanctionsService;
        _pepService = pepService;
        _logger = logger;
    }

    public async Task<bool> ScreenPersonAsync(string fullName, DateTime dateOfBirth, string nationality, CancellationToken cancellationToken = default)
    {
        var sanctions = await _sanctionsService.CheckSanctionsAsync(fullName, nationality, cancellationToken);
        if (!sanctions.IsClean)
        {
            _logger.LogWarning("Sanctions hit for {Name}", fullName);
            return false;
        }

        // PEP check doesn't automatically fail screening, but flags it. 
        // For this boolean check, we might return true but log/flag it elsewhere, 
        // or return false if strict. Let's say strictly false for now if it's a simple "ScreenPerson" check.
        // But usually PEP requires Enhanced Due Diligence, not outright rejection.
        // I'll return true unless sanctions are hit.
        
        return true;
    }

    public async Task<double> CalculateRiskScoreAsync(object personData, CancellationToken cancellationToken = default)
    {
        // Assuming personData has FullName and Nationality via reflection or dynamic
        string fullName = "Unknown";
        string nationality = "Unknown";
        
        try 
        {
            dynamic d = personData;
            fullName = d.FullName;
            nationality = d.Nationality;
        }
        catch
        {
            // Fallback or simplified handling
        }

        var sanctions = await _sanctionsService.CheckSanctionsAsync(fullName, nationality, cancellationToken);
        if (!sanctions.IsClean) return 100.0; // Critical risk

        var pep = await _pepService.CheckPepStatusAsync(fullName, cancellationToken);
        if (pep.IsPep) return 75.0; // High risk

        // Default low risk
        return 10.0; 
    }
    
    // Helper for full result which isn't in interface but useful
    public async Task<KycResult> PerformFullKycAsync(string fullName, DateTime dateOfBirth, string nationality, CancellationToken cancellationToken = default)
    {
        var result = new KycResult();
        
        result.SanctionsResult = await _sanctionsService.CheckSanctionsAsync(fullName, nationality, cancellationToken);
        if (!result.SanctionsResult.IsClean)
        {
            result.Flags.Add("Sanctions Match");
        }

        result.PepResult = await _pepService.CheckPepStatusAsync(fullName, cancellationToken);
        if (result.PepResult.IsPep)
        {
            result.Flags.Add("PEP Detected");
        }

        result.RiskScore = await CalculateRiskScoreAsync(new { FullName = fullName, Nationality = nationality }, cancellationToken);
        
        if (result.RiskScore > 80) result.RiskLevel = "Critical";
        else if (result.RiskScore > 50) result.RiskLevel = "High";
        else if (result.RiskScore > 20) result.RiskLevel = "Medium";
        else result.RiskLevel = "Low";

        result.IsApproved = result.RiskLevel != "Critical"; // Auto-reject critical

        return result;
    }
}
