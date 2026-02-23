using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Pupitre.AI.Core.Orchestration;

#pragma warning disable SKEXP0001

namespace Pupitre.AI.Core.Embeddings;

/// <summary>
/// Default embedding service implementation.
/// </summary>
public sealed class EmbeddingService : IEmbeddingService
{
    private readonly ILLMRouter _router;
    private readonly ILogger<EmbeddingService> _logger;

    public EmbeddingService(ILLMRouter router, ILogger<EmbeddingService> logger)
    {
        _router = router;
        _logger = logger;
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        var kernel = await _router.GetKernelAsync(LLMUseCase.Embedding, cancellationToken);
        var embeddingGenerator = kernel.GetRequiredService<ITextEmbeddingGenerationService>();

        var result = await embeddingGenerator.GenerateEmbeddingAsync(text, kernel, cancellationToken);
        return result.ToArray();
    }

    public async Task<IReadOnlyList<float[]>> GenerateEmbeddingsAsync(
        IEnumerable<string> texts, 
        CancellationToken cancellationToken = default)
    {
        var kernel = await _router.GetKernelAsync(LLMUseCase.Embedding, cancellationToken);
        var embeddingGenerator = kernel.GetRequiredService<ITextEmbeddingGenerationService>();

        var textList = texts.ToList();
        _logger.LogDebug("Generating embeddings for {Count} texts", textList.Count);

        var results = await embeddingGenerator.GenerateEmbeddingsAsync(textList, kernel, cancellationToken);
        return results.Select(r => r.ToArray()).ToList();
    }

    public float CosineSimilarity(float[] embedding1, float[] embedding2)
    {
        if (embedding1.Length != embedding2.Length)
            throw new ArgumentException("Embeddings must have the same dimensions");

        float dotProduct = 0;
        float norm1 = 0;
        float norm2 = 0;

        for (int i = 0; i < embedding1.Length; i++)
        {
            dotProduct += embedding1[i] * embedding2[i];
            norm1 += embedding1[i] * embedding1[i];
            norm2 += embedding2[i] * embedding2[i];
        }

        return dotProduct / (MathF.Sqrt(norm1) * MathF.Sqrt(norm2));
    }
}
