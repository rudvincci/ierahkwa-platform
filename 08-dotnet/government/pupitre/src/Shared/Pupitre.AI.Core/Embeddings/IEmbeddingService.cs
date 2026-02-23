namespace Pupitre.AI.Core.Embeddings;

/// <summary>
/// Service for generating and managing text embeddings.
/// </summary>
public interface IEmbeddingService
{
    /// <summary>
    /// Generates embedding for a single text.
    /// </summary>
    Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates embeddings for multiple texts in batch.
    /// </summary>
    Task<IReadOnlyList<float[]>> GenerateEmbeddingsAsync(
        IEnumerable<string> texts, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates cosine similarity between two embeddings.
    /// </summary>
    float CosineSimilarity(float[] embedding1, float[] embedding2);
}

/// <summary>
/// Represents a text with its embedding.
/// </summary>
public sealed record EmbeddedText
{
    public required string Id { get; init; }
    public required string Text { get; init; }
    public required float[] Embedding { get; init; }
    public Dictionary<string, object>? Metadata { get; init; }
}
