using System.Security.Claims;
using AppBuilder.Core.Interfaces;
using AppBuilder.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBuilder.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PushNotificationsController : ControllerBase
{
    private readonly IPushNotificationService _push;
    private readonly IAppBuilderService _builder;

    public PushNotificationsController(IPushNotificationService push, IAppBuilderService builder)
    {
        _push = push;
        _builder = builder;
    }

    private string? UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    [HttpGet]
    public IActionResult List([FromQuery] string? appProjectId)
    {
        if (!string.IsNullOrEmpty(appProjectId))
        {
            var p = _builder.GetProjectById(appProjectId);
            if (p == null || p.CreatedBy != UserId) return NotFound();
            return Ok(_push.GetByProject(appProjectId));
        }
        return Ok(_push.GetAll().Where(n => _builder.GetProjectById(n.AppProjectId)?.CreatedBy == UserId).ToList());
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreatePushRequest r)
    {
        var p = _builder.GetProjectById(r.AppProjectId);
        if (p == null || p.CreatedBy != UserId) return NotFound(new { error = "Project not found" });
        var n = _push.Create(r.AppProjectId, r.Title, r.Body, r.ImageUrl, r.ScheduledAt, UserId);
        return CreatedAtAction(nameof(Get), new { id = n.Id }, n);
    }

    [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        var n = _push.GetById(id);
        if (n == null) return NotFound();
        var p = _builder.GetProjectById(n.AppProjectId);
        if (p?.CreatedBy != UserId) return NotFound();
        return Ok(n);
    }
}

public class CreatePushRequest
{
    public string AppProjectId { get; set; } = "";
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public string? ImageUrl { get; set; }
    public DateTime? ScheduledAt { get; set; }
}
