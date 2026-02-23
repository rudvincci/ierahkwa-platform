using AppBuilder.Core.Interfaces;
using AppBuilder.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBuilder.Infrastructure.Services;

/// <summary>AI assistant – Appy: Guided app creation, website analysis. OpenAI or Anthropic. Mock when no key.</summary>
public class AiAssistantService : IAiAssistantService
{
    private readonly IConfiguration _config;
    private readonly ILogger<AiAssistantService> _log;

    public AiAssistantService(IConfiguration config, ILogger<AiAssistantService> log)
    {
        _config = config;
        _log = log;
    }

    public AiProvider GetConfiguredProvider()
    {
        var k = _config["Ai:Provider"] ?? "OpenAI";
        return Enum.TryParse<AiProvider>(k, true, out var p) ? p : AiProvider.OpenAI;
    }

    public string SendMessage(string sessionId, string userMessage, string? websiteUrl = null, object? currentAppConfig = null)
    {
        var hasOpenAi = !string.IsNullOrEmpty(_config["Ai:OpenAI:ApiKey"]);
        var hasAnthropic = !string.IsNullOrEmpty(_config["Ai:Anthropic:ApiKey"]);

        if (hasOpenAi || hasAnthropic)
        {
            // TODO: call OpenAI or Anthropic API. For now fallback to mock.
            _log.LogDebug("AI: keys configured but using mock response for session {Session}", sessionId);
        }

        // Mock: suggest config from website URL and common patterns
        var url = websiteUrl ?? "";
        var lower = userMessage.ToLowerInvariant();
        if (lower.Contains("color") || lower.Contains("theme"))
            return "For a professional look, use Primary **#1a237e** (Ierahkwa blue) and **#ffd700** (gold). For dark mode set Background to **#0d1117**.";
        if (lower.Contains("icon") || lower.Contains("logo"))
            return "Use a 512×512 PNG for the app icon and 192×192 for the status bar. Transparent or solid background works best.";
        if (lower.Contains("navigation") || lower.Contains("menu"))
            return "**WebView** (default): single page, good for most sites. **Tabs**: if your site has 3–5 main sections. **Drawer**: for many sections.";
        if (lower.Contains("website") || lower.Contains("url") || !string.IsNullOrWhiteSpace(url))
            return "I’ve taken your URL into account. Suggested: display=standalone, orientation=any, theme_color from your brand. Enable **Push notifications** if you have a Firebase project.";
        return "I can help with: **colors**, **icons**, **navigation style**, or **website-based setup**. What would you like to configure?";
    }
}
