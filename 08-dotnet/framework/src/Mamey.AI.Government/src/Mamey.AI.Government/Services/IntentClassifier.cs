using Microsoft.Extensions.Logging;
using Mamey.AI.Government.Models;

namespace Mamey.AI.Government.Services;

public class IntentClassifier
{
    private readonly ILogger<IntentClassifier> _logger;

    public IntentClassifier(ILogger<IntentClassifier> logger)
    {
        _logger = logger;
    }

    public async Task<IntentResult> ClassifyAsync(string text, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Classifying intent for message: {Message}...", text.Length > 20 ? text[..20] + "..." : text);
        await Task.Delay(50, cancellationToken);

        // Mock: Simple keyword matching
        var result = new IntentResult();
        var lower = text.ToLower();

        if (lower.Contains("passport")) result.Intent = "PassportService";
        else if (lower.Contains("status") || lower.Contains("track")) result.Intent = "CheckStatus";
        else if (lower.Contains("apply") || lower.Contains("application")) result.Intent = "NewApplication";
        else if (lower.Contains("help") || lower.Contains("human") || lower.Contains("agent")) result.Intent = "HumanHandoff";
        else result.Intent = "GeneralInquiry";

        result.Confidence = 0.85;
        return result;
    }
}
