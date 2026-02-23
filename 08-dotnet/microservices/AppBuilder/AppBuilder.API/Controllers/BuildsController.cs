using System.Security.Claims;
using AppBuilder.Core.Interfaces;
using AppBuilder.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppBuilder.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BuildsController : ControllerBase
{
    private readonly IAppBuilderService _service;
    private readonly ISubscriptionService _subs;
    private readonly ILogger<BuildsController> _logger;

    public BuildsController(IAppBuilderService service, ISubscriptionService subs, ILogger<BuildsController> logger)
    {
        _service = service;
        _subs = subs;
        _logger = logger;
    }

    [HttpGet("project/{projectId}")]
    public IActionResult GetByProject(string projectId) => Ok(_service.GetBuildsByProject(projectId));

    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        var b = _service.GetBuildById(id);
        if (b == null) return NotFound(new { error = "Build not found" });
        return Ok(b);
    }

    [HttpPost]
    public IActionResult Create([FromBody] BuildRequest request)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(uid) && !_subs.ConsumeBuildCredit(uid))
                return StatusCode(402, new { error = "No build credits. Upgrade your plan or wait for next period." });
        }
        try
        {
            var build = _service.CreateBuild(request);
            return CreatedAtAction(nameof(GetById), new { id = build.Id }, build);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}/download")]
    public IActionResult Download(string id, [FromQuery] string? platform)
    {
        var b = _service.GetBuildById(id);
        if (b == null || b.Status != BuildStatus.Success)
            return NotFound(new { error = "Build not found or not ready" });

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var downloadUrl = $"{baseUrl}/api/builds/{id}/download?platform={b.Platform}";
        return Ok(new
        {
            buildId = b.Id,
            platform = b.Platform.ToString(),
            version = b.Version,
            downloadUrl,
            qrCodeUrl = $"{baseUrl}/api/builds/{id}/qr",
            message = "In production, Capacitor/Tauri would provide the actual .apk, .exe, .dmg, or .AppImage."
        });
    }

    [HttpGet("{id}/qr")]
    public IActionResult Qr(string id)
    {
        var b = _service.GetBuildById(id);
        if (b == null) return NotFound(new { error = "Build not found" });
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var downloadUrl = $"{baseUrl}/api/builds/{id}/download?platform={b.Platform}";
        var qrImageUrl = "https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=" + Uri.EscapeDataString(downloadUrl);
        return Ok(new { qrImageUrl, downloadUrl });
    }
}
