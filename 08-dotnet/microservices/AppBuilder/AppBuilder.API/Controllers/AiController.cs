using AppBuilder.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AppBuilder.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly IAiAssistantService _ai;

    public AiController(IAiAssistantService ai) => _ai = ai;

    [HttpPost("chat")]
    public IActionResult Chat([FromBody] AiChatRequest r)
    {
        var sessionId = r.SessionId ?? Guid.NewGuid().ToString();
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
