namespace AppBuilder.Core.Models;

/// <summary>AI chat message - Appy: AI-Powered App Configuration. OpenAI / Anthropic. Guided setup, website analysis.</summary>
public class AiChatMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string SessionId { get; set; } = string.Empty;
    public string Role { get; set; } = "user";   // user, assistant
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>AI provider: OpenAI (GPT) or Anthropic (Claude).</summary>
public enum AiProvider
{
    OpenAI,
    Anthropic
}
