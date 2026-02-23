using Microsoft.SemanticKernel;

namespace Pupitre.AI.Core.Orchestration;

/// <summary>
/// Routes requests to appropriate LLM providers based on context.
/// </summary>
public interface ILLMRouter
{
    /// <summary>
    /// Gets a kernel configured for the specified use case.
    /// </summary>
    /// <param name="useCase">The intended use case (tutoring, assessment, content, etc.).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Configured Semantic Kernel instance.</returns>
    Task<Kernel> GetKernelAsync(LLMUseCase useCase, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the best provider for a given use case.
    /// </summary>
    /// <param name="useCase">The intended use case.</param>
    /// <returns>Provider identifier.</returns>
    string GetPreferredProvider(LLMUseCase useCase);
}

/// <summary>
/// LLM use cases for intelligent routing.
/// </summary>
public enum LLMUseCase
{
    /// <summary>Interactive tutoring - requires low latency.</summary>
    Tutoring,
    
    /// <summary>Assessment generation and grading.</summary>
    Assessment,
    
    /// <summary>Content generation (lessons, explanations).</summary>
    ContentGeneration,
    
    /// <summary>Speech-to-text transcription.</summary>
    SpeechToText,
    
    /// <summary>Text-to-speech synthesis.</summary>
    TextToSpeech,
    
    /// <summary>Content safety moderation.</summary>
    SafetyModeration,
    
    /// <summary>Text translation.</summary>
    Translation,
    
    /// <summary>Image/document analysis.</summary>
    VisionAnalysis,
    
    /// <summary>Recommendation engine.</summary>
    Recommendations,
    
    /// <summary>Behavioral analysis.</summary>
    BehaviorAnalysis,
    
    /// <summary>Embedding generation.</summary>
    Embedding
}
