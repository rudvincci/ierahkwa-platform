namespace Pupitre.AI.Core.Options;

/// <summary>
/// Configuration options for AI services.
/// </summary>
public sealed class AIOptions
{
    public const string SectionName = "AI";

    /// <summary>
    /// Default LLM provider (OpenAI, AzureOpenAI, Ollama, Anthropic).
    /// </summary>
    public string DefaultProvider { get; set; } = "AzureOpenAI";

    /// <summary>
    /// OpenAI configuration.
    /// </summary>
    public OpenAIOptions? OpenAI { get; set; }

    /// <summary>
    /// Azure OpenAI configuration.
    /// </summary>
    public AzureOpenAIOptions? AzureOpenAI { get; set; }

    /// <summary>
    /// Ollama configuration for local models.
    /// </summary>
    public OllamaOptions? Ollama { get; set; }

    /// <summary>
    /// Qdrant vector database configuration.
    /// </summary>
    public QdrantOptions Qdrant { get; set; } = new();

    /// <summary>
    /// Embedding configuration.
    /// </summary>
    public EmbeddingOptions Embedding { get; set; } = new();
}

public sealed class OpenAIOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string ChatModel { get; set; } = "gpt-4-turbo";
    public string EmbeddingModel { get; set; } = "text-embedding-3-small";
    public string Organization { get; set; } = string.Empty;
}

public sealed class AzureOpenAIOptions
{
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ChatDeployment { get; set; } = "gpt-4-turbo";
    public string EmbeddingDeployment { get; set; } = "text-embedding-ada-002";
    public string ApiVersion { get; set; } = "2024-02-01";
}

public sealed class OllamaOptions
{
    public string Endpoint { get; set; } = "http://localhost:11434";
    public string ChatModel { get; set; } = "llama3";
    public string EmbeddingModel { get; set; } = "nomic-embed-text";
}

public sealed class QdrantOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 6333;
    public int GrpcPort { get; set; } = 6334;
    public string ApiKey { get; set; } = string.Empty;
    public bool UseTls { get; set; } = false;
}

public sealed class EmbeddingOptions
{
    public int Dimensions { get; set; } = 1536;
    public int BatchSize { get; set; } = 100;
}
