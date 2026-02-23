using AppBuilder.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBuilder.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AiController : ControllerBase
{
    private readonly IAiAssistantService _ai;

    public AiController(IAiAssistantService ai) => _ai = ai;

    [HttpPost("chat")]
    public IActionResult Chat([FromBody] AiChatRequest r)
    {
        var sessionId = r.SessionId ?? Guid.NewGuid().ToString();
        if (!string.IsNullOrEmpty(r.WebsiteUrl) && !Uri.TryCreate(r.WebsiteUrl, UriKind.Absolute, out var uri))
            return BadRequest(new { error = "Invalid URL format" });
        if (string.IsNullOrWhiteSpace(r.Message) || r.Message.Length > 10000)
            return BadRequest(new { error = "Message is required and must be under 10,000 characters" });
        var response = _ai.SendMessage(sessionId, r.Message, r.WebsiteUrl, r.CurrentAppConfig);
        return Ok(new { sessionId, response, provider = _ai.GetConfiguredProvider().ToString() });
    }

    [HttpGet("provider")]
    public IActionResult Provider() => Ok(new { provider = _ai.GetConfiguredProvider().ToString() });
}

public class AiChatRequest
{
    public string? SessionId { get; set; }
    public string Message { get; set; } = "";
    public string? WebsiteUrl { get; set; }
    public object? CurrentAppConfig { get; set; }
}
