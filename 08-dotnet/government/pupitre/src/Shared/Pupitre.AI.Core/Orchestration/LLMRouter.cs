using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Pupitre.AI.Core.Options;

namespace Pupitre.AI.Core.Orchestration;

/// <summary>
/// Default implementation of LLM router.
/// </summary>
public sealed class LLMRouter : ILLMRouter
{
    private readonly AIOptions _options;
    private readonly ILogger<LLMRouter> _logger;

    public LLMRouter(IOptions<AIOptions> options, ILogger<LLMRouter> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task<Kernel> GetKernelAsync(LLMUseCase useCase, CancellationToken cancellationToken = default)
    {
        var provider = GetPreferredProvider(useCase);
        _logger.LogDebug("Creating kernel for use case {UseCase} using provider {Provider}", useCase, provider);

        var builder = Kernel.CreateBuilder();

        switch (provider.ToLowerInvariant())
        {
            case "azureopenai":
                ConfigureAzureOpenAI(builder, useCase);
                break;
            case "openai":
                ConfigureOpenAI(builder, useCase);
                break;
            case "ollama":
                ConfigureOllama(builder, useCase);
                break;
            default:
                throw new InvalidOperationException($"Unknown provider: {provider}");
        }

        return Task.FromResult(builder.Build());
    }

    public string GetPreferredProvider(LLMUseCase useCase)
    {
        // Route based on use case requirements
        return useCase switch
        {
            // Use Ollama for local, low-latency interactions when available
            LLMUseCase.Tutoring when _options.Ollama is not null => "Ollama",
            
            // Vision requires specific models
            LLMUseCase.VisionAnalysis => _options.DefaultProvider,
            
            // Embeddings - prefer local for cost savings
            LLMUseCase.Embedding when _options.Ollama is not null => "Ollama",
            
            // Default to configured provider
            _ => _options.DefaultProvider
        };
    }

    private void ConfigureAzureOpenAI(IKernelBuilder builder, LLMUseCase useCase)
    {
        var azure = _options.AzureOpenAI ?? throw new InvalidOperationException("Azure OpenAI not configured");

        if (useCase == LLMUseCase.Embedding)
        {
            builder.AddAzureOpenAITextEmbeddingGeneration(
                deploymentName: azure.EmbeddingDeployment,
                endpoint: azure.Endpoint,
                apiKey: azure.ApiKey);
        }
        else
        {
            builder.AddAzureOpenAIChatCompletion(
                deploymentName: azure.ChatDeployment,
                endpoint: azure.Endpoint,
                apiKey: azure.ApiKey);
        }
    }

    private void ConfigureOpenAI(IKernelBuilder builder, LLMUseCase useCase)
    {
        var openai = _options.OpenAI ?? throw new InvalidOperationException("OpenAI not configured");

        if (useCase == LLMUseCase.Embedding)
        {
            builder.AddOpenAITextEmbeddingGeneration(
                modelId: openai.EmbeddingModel,
                apiKey: openai.ApiKey);
        }
        else
        {
            builder.AddOpenAIChatCompletion(
                modelId: openai.ChatModel,
                apiKey: openai.ApiKey);
        }
    }

    private void ConfigureOllama(IKernelBuilder builder, LLMUseCase useCase)
    {
        var ollama = _options.Ollama ?? throw new InvalidOperationException("Ollama not configured");

        // Ollama uses OpenAI-compatible API
        if (useCase == LLMUseCase.Embedding)
        {
            builder.AddOpenAITextEmbeddingGeneration(
                modelId: ollama.EmbeddingModel,
                apiKey: "ollama"); // Ollama doesn't require API key
        }
        else
        {
            builder.AddOpenAIChatCompletion(
                modelId: ollama.ChatModel,
                endpoint: new Uri($"{ollama.Endpoint}/v1"),
                apiKey: "ollama");
        }
    }
}
