using System.Security.Claims;
using AppBuilder.Core.Interfaces;
using AppBuilder.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBuilder.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IAppBuilderService _service;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(IAppBuilderService service, ILogger<ProjectsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] bool my = false)
    {
        if (my && User.Identity?.IsAuthenticated == true)
            return Ok(_service.GetProjectsByUser(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
        return Ok(_service.GetAllProjects());
    }

    [HttpPost("from-url")]
    public IActionResult CreateFromUrl([FromBody] CreateFromUrlRequest r)
    {
        var userId = User.Identity?.IsAuthenticated == true ? User.FindFirst(ClaimTypes.NameIdentifier)?.Value : null;
        var p = new AppProject
        {
            Name = r.Name ?? "My App",
            SourceUrl = r.Url ?? "",
            CreatedBy = userId,
            Design = new AppDesign { PrimaryColor = "#1a237e", SecondaryColor = "#ffd700" }
        };
        var created = _service.CreateProject(p);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        var p = _service.GetProjectById(id);
        if (p == null) return NotFound(new { error = "Project not found" });
        return Ok(p);
    }

    [HttpPost]
    public IActionResult Create([FromBody] AppProject project)
    {
        if (User.Identity?.IsAuthenticated == true)
            project.CreatedBy ??= User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var created = _service.CreateProject(project);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] AppProject project)
    {
        var updated = _service.UpdateProject(id, project);
        if (updated == null) return NotFound(new { error = "Project not found" });
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        if (!_service.DeleteProject(id)) return NotFound(new { error = "Project not found" });
        return NoContent();
    }

    [HttpGet("{id}/config/capacitor")]
    public IActionResult GetCapacitorConfig(string id)
    {
        var p = _service.GetProjectById(id);
        if (p == null) return NotFound(new { error = "Project not found" });
        return Content(_service.GenerateCapacitorConfig(p), "application/json");
    }

    [HttpGet("{id}/config/pwa-manifest")]
    public IActionResult GetPwaManifest(string id)
    {
        var p = _service.GetProjectById(id);
        if (p == null) return NotFound(new { error = "Project not found" });
        return Content(_service.GeneratePwaManifest(p), "application/json");
    }
}

public class CreateFromUrlRequest { public string? Url { get; set; } public string? Name { get; set; } }
