using Mamey.AI.Government.Interfaces;
using Mamey.AI.Government.Models;
using Microsoft.Extensions.Logging;

namespace Mamey.AI.Government.Services;

public class ChatbotService : INLPService
{
    private readonly IntentClassifier _classifier;
    private readonly ResponseGenerator _generator;
    private readonly ILogger<ChatbotService> _logger;

    public ChatbotService(
        IntentClassifier classifier,
        ResponseGenerator generator,
        ILogger<ChatbotService> logger)
    {
        _classifier = classifier;
        _generator = generator;
        _logger = logger;
    }

    public async Task<IntentResult> ClassifyIntentAsync(string message, CancellationToken cancellationToken = default)
    {
        return await _classifier.ClassifyAsync(message, cancellationToken);
    }

    public async Task<ChatResponse> GenerateResponseAsync(ChatRequest request, CancellationToken cancellationToken = default)
    {
        var intentResult = await _classifier.ClassifyAsync(request.Message, cancellationToken);
        return await _generator.GenerateAsync(intentResult.Intent, request, cancellationToken);
    }
}
