using Microsoft.AspNetCore.Mvc;
using IERAHKWA.Platform.Models;
using IERAHKWA.Platform.Services;

namespace IERAHKWA.Platform.Controllers;

[ApiController]
[Route("api/ai")]
public class AIController : ControllerBase
{
    private readonly IAIService _aiService;
    private readonly ILogger<AIController> _logger;

    public AIController(IAIService aiService, ILogger<AIController> logger)
    {
        _aiService = aiService;
        _logger = logger;
    }

    [HttpPost("chat")]
    public async Task<ActionResult<ApiResponse<object>>> Chat([FromBody] ChatRequest request)
    {
        try
        {
            var response = await _aiService.ChatAsync(request.Message ?? "", request.Context);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { response = response, message = response }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AI chat");
            return StatusCode(500, new ApiResponse<object> { Success = false, Error = ex.Message });
        }
    }

    [HttpPost("code/generate")]
    public async Task<ActionResult<ApiResponse<object>>> GenerateCode([FromBody] CodeGenerateRequest request)
    {
        try
        {
            var code = await _aiService.GenerateCodeAsync(request.Prompt ?? "", request.Language ?? "javascript");
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { code = code, language = request.Language ?? "javascript" }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating code");
            return StatusCode(500, new ApiResponse<object> { Success = false, Error = ex.Message });
        }
    }

    [HttpPost("analyze")]
    public async Task<ActionResult<ApiResponse<object>>> AnalyzeCode([FromBody] CodeAnalyzeRequest request)
    {
        try
        {
            var analysis = await _aiService.AnalyzeCodeAsync(request.Code ?? "");
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { analysis = analysis }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing code");
            return StatusCode(500, new ApiResponse<object> { Success = false, Error = ex.Message });
        }
    }
}

public class ChatRequest
{
    public string? Message { get; set; }
    public string? Context { get; set; }
}

public class CodeGenerateRequest
{
    public string? Prompt { get; set; }
    public string? Language { get; set; }
}

public class CodeAnalyzeRequest
{
    public string? Code { get; set; }
}
