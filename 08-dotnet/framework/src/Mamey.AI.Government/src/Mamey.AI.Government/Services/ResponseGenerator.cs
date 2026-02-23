using Microsoft.Extensions.Logging;
using Mamey.AI.Government.Models;

namespace Mamey.AI.Government.Services;

public class ResponseGenerator
{
    private readonly ILogger<ResponseGenerator> _logger;

    public ResponseGenerator(ILogger<ResponseGenerator> logger)
    {
        _logger = logger;
    }

    public async Task<ChatResponse> GenerateAsync(string intent, ChatRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating response for intent: {Intent}...", intent);
        await Task.Delay(50, cancellationToken);

        var response = new ChatResponse { Confidence = 0.9 };

        switch (intent)
        {
            case "PassportService":
                response.Message = "I can help you with passport services. Would you like to apply for a new passport or renew an existing one?";
                response.Suggestions.Add("Apply for Passport");
                response.Suggestions.Add("Renew Passport");
                break;
            case "CheckStatus":
                response.Message = "Please provide your Application ID to check the status.";
                break;
            case "HumanHandoff":
                response.Message = "I will connect you with a support agent. Please wait a moment.";
                response.ShouldHandoff = true;
                break;
            default:
                response.Message = "I'm not sure I understand. Can you rephrase or choose from the options below?";
                response.Suggestions.Add("Passport Services");
                response.Suggestions.Add("Check Status");
                response.Suggestions.Add("Speak to Agent");
                break;
        }

        return response;
    }
}
