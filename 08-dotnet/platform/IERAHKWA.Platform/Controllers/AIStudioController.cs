using Microsoft.AspNetCore.Mvc;
using IERAHKWA.Platform.Services;
using IERAHKWA.Platform.Models;

namespace IERAHKWA.Platform.Controllers;

[ApiController]
[Route("api/ai-studio")]
public class AIStudioController : ControllerBase
{
    private readonly IAIStudioService _studioService;
    private readonly ILogger<AIStudioController> _logger;

    public AIStudioController(IAIStudioService studioService, ILogger<AIStudioController> logger)
    {
        _studioService = studioService;
        _logger = logger;
    }

    [HttpPost("code/generate")]
    public async Task<IActionResult> GenerateCode([FromBody] Models.CodeGenerateRequest request)
    {
        try
        {
            var result = await _studioService.GenerateCodeAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating code");
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    [HttpPost("web/generate")]
    public async Task<IActionResult> GenerateWebsite([FromBody] Models.WebGenerateRequest request)
    {
        try
        {
            var result = await _studioService.GenerateWebsiteAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating website");
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    [HttpPost("app/generate")]
    public async Task<IActionResult> GenerateApp([FromBody] Models.AppGenerateRequest request)
    {
        try
        {
            var result = await _studioService.GenerateAppAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating app");
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    [HttpPost("api/generate")]
    public async Task<IActionResult> GenerateAPI([FromBody] Models.APIGenerateRequest request)
    {
        try
        {
            var result = await _studioService.GenerateAPIAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating API");
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    [HttpPost("bot/generate")]
    public async Task<IActionResult> GenerateBot([FromBody] Models.BotGenerateRequest request)
    {
        try
        {
            var result = await _studioService.GenerateBotAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating bot");
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    [HttpPost("document/analyze")]
    public async Task<IActionResult> AnalyzeDocument([FromBody] Models.DocumentAnalyzeRequest request)
    {
        try
        {
            var result = await _studioService.AnalyzeDocumentAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing document");
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    [HttpPost("blockchain/contract")]
    public async Task<IActionResult> GenerateSmartContract([FromBody] Models.SmartContractRequest request)
    {
        try
        {
            var result = await _studioService.GenerateSmartContractAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating smart contract");
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    [HttpGet("templates")]
    public async Task<IActionResult> GetTemplates([FromQuery] string? category = null)
    {
        var templates = await _studioService.GetTemplatesAsync(category);
        return Ok(new { success = true, data = templates });
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _studioService.GetStatsAsync();
        return Ok(new { success = true, data = stats });
    }
}
