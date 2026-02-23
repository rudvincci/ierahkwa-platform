using Mamey.AI.Government.Models;

namespace Mamey.AI.Government.Interfaces;

public interface INLPService
{
    Task<IntentResult> ClassifyIntentAsync(string message, CancellationToken cancellationToken = default);
    Task<ChatResponse> GenerateResponseAsync(ChatRequest request, CancellationToken cancellationToken = default);
}
