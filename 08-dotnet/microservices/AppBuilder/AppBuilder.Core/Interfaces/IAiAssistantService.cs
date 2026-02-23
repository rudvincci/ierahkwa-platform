using AppBuilder.Core.Models;

namespace AppBuilder.Core.Interfaces;

/// <summary>AI assistant - Appy: Guided app creation, auto setup, website analysis. OpenAI or Anthropic.</summary>
public interface IAiAssistantService
{
    string SendMessage(string sessionId, string userMessage, string? websiteUrl = null, object? currentAppConfig = null);
    AiProvider GetConfiguredProvider();
}
